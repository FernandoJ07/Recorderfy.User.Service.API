-- Script de Validación y Corrección de Base de Datos
-- Recorderfy User Service
-- PostgreSQL 12+

-- =====================================================
-- VERIFICACIÓN DE ESTRUCTURA
-- =====================================================

-- Activar extensión pgcrypto (para gen_random_uuid)
CREATE EXTENSION IF NOT EXISTS pgcrypto;

-- =====================================================
-- TABLA: tipo_documento
-- =====================================================
DO $$ 
BEGIN
    IF NOT EXISTS (SELECT FROM pg_tables WHERE schemaname = 'public' AND tablename = 'tipo_documento') THEN
        CREATE TABLE tipo_documento (
            id_tipo_documento SERIAL PRIMARY KEY,
            nombre_documento VARCHAR(50) NOT NULL UNIQUE,
            descripcion TEXT
        );
        
        -- Insertar tipos de documento comunes
        INSERT INTO tipo_documento (nombre_documento, descripcion) VALUES
        ('DNI', 'Documento Nacional de Identidad'),
        ('Pasaporte', 'Pasaporte'),
        ('Cédula', 'Cédula de Ciudadanía'),
        ('RUC', 'Registro Único de Contribuyentes');
    END IF;
END $$;

-- =====================================================
-- TABLA: rol
-- =====================================================
DO $$ 
BEGIN
    IF NOT EXISTS (SELECT FROM pg_tables WHERE schemaname = 'public' AND tablename = 'rol') THEN
        CREATE TABLE rol (
            id_rol SERIAL PRIMARY KEY,
            nombre_rol VARCHAR(50) NOT NULL UNIQUE
        );
        
        -- Insertar roles básicos
        INSERT INTO rol (nombre_rol) VALUES
        ('Administrador'),
        ('Médico'),
        ('Paciente'),
        ('Cuidador');
    END IF;
END $$;

-- =====================================================
-- TABLA: usuario
-- =====================================================
DO $$ 
BEGIN
    IF NOT EXISTS (SELECT FROM pg_tables WHERE schemaname = 'public' AND tablename = 'usuario') THEN
        CREATE TABLE usuario (
            id_usuario UUID PRIMARY KEY DEFAULT gen_random_uuid(),
            nombre VARCHAR(100) NOT NULL,
            apellido VARCHAR(100) NOT NULL,
            id_tipo_documento INTEGER REFERENCES tipo_documento(id_tipo_documento) ON DELETE RESTRICT,
            nro_documento VARCHAR(50) NOT NULL UNIQUE,
            email VARCHAR(150) NOT NULL UNIQUE,
            telefono VARCHAR(30),
            password_hash VARCHAR(255) NOT NULL,
            id_rol INTEGER REFERENCES rol(id_rol) ON DELETE RESTRICT,
            genero CHAR(1) CHECK (genero IN ('M', 'F', 'O')),
            fecha_nacimiento DATE,
            fecha_registro TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
            ultimo_acceso TIMESTAMP WITHOUT TIME ZONE,
            estado VARCHAR(20) DEFAULT 'pendiente' CHECK (estado IN ('pendiente', 'activo', 'inactivo', 'bloqueado')),
            foto_perfil TEXT
        );
        
        -- Índices para mejorar rendimiento
        CREATE INDEX idx_usuarios_documento ON usuario(nro_documento);
        CREATE INDEX idx_usuarios_email ON usuario(email);
    END IF;
END $$;

-- =====================================================
-- TABLA: cuidador
-- =====================================================
DO $$ 
BEGIN
    IF NOT EXISTS (SELECT FROM pg_tables WHERE schemaname = 'public' AND tablename = 'cuidador') THEN
        CREATE TABLE cuidador (
            id_usuario UUID PRIMARY KEY REFERENCES usuario(id_usuario) ON DELETE CASCADE,
            relacion_con_paciente VARCHAR(50),
            direccion VARCHAR(200),
            pacientes_asociados JSONB DEFAULT '[]'::jsonb,
            notificaciones_activadas BOOLEAN DEFAULT TRUE
        );
    ELSE
        -- Verificar y agregar PK si no existe
        IF NOT EXISTS (
            SELECT 1 FROM information_schema.table_constraints 
            WHERE table_name = 'cuidador' AND constraint_type = 'PRIMARY KEY'
        ) THEN
            ALTER TABLE cuidador ADD PRIMARY KEY (id_usuario);
        END IF;
        
        -- Verificar y agregar FK si no existe
        IF NOT EXISTS (
            SELECT 1 FROM information_schema.table_constraints 
            WHERE table_name = 'cuidador' AND constraint_name = 'cuidador_id_usuario_fkey'
        ) THEN
            ALTER TABLE cuidador ADD CONSTRAINT cuidador_id_usuario_fkey 
                FOREIGN KEY (id_usuario) REFERENCES usuario(id_usuario) ON DELETE CASCADE;
        END IF;
    END IF;
END $$;

-- =====================================================
-- TABLA: medico
-- =====================================================
DO $$ 
BEGIN
    IF NOT EXISTS (SELECT FROM pg_tables WHERE schemaname = 'public' AND tablename = 'medico') THEN
        CREATE TABLE medico (
            id_usuario UUID PRIMARY KEY REFERENCES usuario(id_usuario) ON DELETE CASCADE,
            especialidad VARCHAR(100),
            centro_medico VARCHAR(150),
            pacientes_asignados JSONB DEFAULT '[]'::jsonb,
            notificaciones_activadas BOOLEAN DEFAULT TRUE,
            firma_digital TEXT
        );
    ELSE
        -- Verificar y agregar PK si no existe
        IF NOT EXISTS (
            SELECT 1 FROM information_schema.table_constraints 
            WHERE table_name = 'medico' AND constraint_type = 'PRIMARY KEY'
        ) THEN
            ALTER TABLE medico ADD PRIMARY KEY (id_usuario);
        END IF;
        
        -- Verificar y agregar FK si no existe
        IF NOT EXISTS (
            SELECT 1 FROM information_schema.table_constraints 
            WHERE table_name = 'medico' AND constraint_name = 'medico_id_usuario_fkey'
        ) THEN
            ALTER TABLE medico ADD CONSTRAINT medico_id_usuario_fkey 
                FOREIGN KEY (id_usuario) REFERENCES usuario(id_usuario) ON DELETE CASCADE;
        END IF;
    END IF;
END $$;

-- =====================================================
-- TABLA: paciente
-- =====================================================
DO $$ 
BEGIN
    IF NOT EXISTS (SELECT FROM pg_tables WHERE schemaname = 'public' AND tablename = 'paciente') THEN
        CREATE TABLE paciente (
            id_usuario UUID PRIMARY KEY REFERENCES usuario(id_usuario) ON DELETE CASCADE,
            diagnostico_inicial VARCHAR(150),
            fecha_ingreso DATE DEFAULT CURRENT_DATE,
            id_cuidador UUID REFERENCES cuidador(id_usuario) ON DELETE SET NULL,
            id_medico UUID REFERENCES medico(id_usuario) ON DELETE SET NULL,
            observaciones_clinicas TEXT,
            foto_referencia TEXT
        );
        
        -- Índices para mejorar rendimiento en búsquedas
        CREATE INDEX idx_pacientes_cuidador ON paciente(id_cuidador);
        CREATE INDEX idx_pacientes_medico ON paciente(id_medico);
    ELSE
        -- Verificar y agregar PK si no existe
        IF NOT EXISTS (
            SELECT 1 FROM information_schema.table_constraints 
            WHERE table_name = 'paciente' AND constraint_type = 'PRIMARY KEY'
        ) THEN
            ALTER TABLE paciente ADD PRIMARY KEY (id_usuario);
        END IF;
        
        -- Verificar y agregar FK si no existe
        IF NOT EXISTS (
            SELECT 1 FROM information_schema.table_constraints 
            WHERE table_name = 'paciente' AND constraint_name = 'paciente_id_usuario_fkey'
        ) THEN
            ALTER TABLE paciente ADD CONSTRAINT paciente_id_usuario_fkey 
                FOREIGN KEY (id_usuario) REFERENCES usuario(id_usuario) ON DELETE CASCADE;
        END IF;
        
        IF NOT EXISTS (
            SELECT 1 FROM information_schema.table_constraints 
            WHERE table_name = 'paciente' AND constraint_name = 'paciente_id_cuidador_fkey'
        ) THEN
            ALTER TABLE paciente ADD CONSTRAINT paciente_id_cuidador_fkey 
                FOREIGN KEY (id_cuidador) REFERENCES cuidador(id_usuario) ON DELETE SET NULL;
        END IF;
        
        IF NOT EXISTS (
            SELECT 1 FROM information_schema.table_constraints 
            WHERE table_name = 'paciente' AND constraint_name = 'paciente_id_medico_fkey'
        ) THEN
            ALTER TABLE paciente ADD CONSTRAINT paciente_id_medico_fkey 
                FOREIGN KEY (id_medico) REFERENCES medico(id_usuario) ON DELETE SET NULL;
        END IF;
    END IF;
END $$;

-- =====================================================
-- VERIFICACIÓN FINAL
-- =====================================================

-- Mostrar todas las tablas creadas
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public' 
  AND table_type = 'BASE TABLE'
ORDER BY table_name;

-- Mostrar todas las claves primarias
SELECT 
    tc.table_name, 
    kcu.column_name
FROM information_schema.table_constraints tc
JOIN information_schema.key_column_usage kcu 
  ON tc.constraint_name = kcu.constraint_name
WHERE tc.constraint_type = 'PRIMARY KEY'
  AND tc.table_schema = 'public'
ORDER BY tc.table_name;

-- Mostrar todas las claves foráneas
SELECT 
    tc.table_name AS tabla_origen, 
    kcu.column_name AS columna_origen, 
    ccu.table_name AS tabla_destino,
    ccu.column_name AS columna_destino,
    rc.delete_rule AS regla_delete
FROM information_schema.table_constraints AS tc 
JOIN information_schema.key_column_usage AS kcu
  ON tc.constraint_name = kcu.constraint_name
JOIN information_schema.constraint_column_usage AS ccu
  ON ccu.constraint_name = tc.constraint_name
JOIN information_schema.referential_constraints AS rc
  ON rc.constraint_name = tc.constraint_name
WHERE tc.constraint_type = 'FOREIGN KEY'
  AND tc.table_schema = 'public'
ORDER BY tc.table_name;
