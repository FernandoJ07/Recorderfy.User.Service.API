DROP TABLE IF EXISTS paciente, medico, cuidador CASCADE;
DROP TABLE IF EXISTS usuario, tipo_documento, rol CASCADE;

CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Roles
CREATE TABLE rol (
    id_rol SERIAL PRIMARY KEY,
    nombre_rol VARCHAR(50) UNIQUE NOT NULL
);

INSERT INTO rol (nombre_rol) VALUES ('medico'), ('paciente'), ('familiar')
ON CONFLICT DO NOTHING;

-- Tipo documento
CREATE TABLE tipo_documento (
    id_tipo_documento SERIAL PRIMARY KEY,
    nombre_documento VARCHAR(50) UNIQUE NOT NULL,
    descripcion TEXT
);

INSERT INTO tipo_documento (nombre_documento, descripcion)
VALUES ('CC','Cédula'),
       ('TI','Tarjeta de Identidad'),
       ('CE','Cédula de Extranjería'),
       ('PA','Pasaporte')
ON CONFLICT DO NOTHING;

-- Usuario
CREATE TABLE usuario (
    id_usuario UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    id_tipo_documento INT REFERENCES tipo_documento(id_tipo_documento),
    nro_documento VARCHAR(50) UNIQUE NOT NULL,
    email VARCHAR(150) UNIQUE NOT NULL,
    telefono VARCHAR(30),
    password_hash VARCHAR(255) NOT NULL,
    id_rol INT REFERENCES rol(id_rol),
    genero CHAR(1) CHECK (genero IN ('M','F')),
    fecha_nacimiento DATE,
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ultimo_acceso TIMESTAMP,
    estado VARCHAR(20) DEFAULT 'pendiente',
    foto_perfil TEXT
);

-- Cuidador
CREATE TABLE cuidador (
    id_usuario UUID PRIMARY KEY REFERENCES usuario(id_usuario) ON DELETE CASCADE,
    relacion_con_paciente VARCHAR(50),
    direccion VARCHAR(200),
    pacientes_asociados JSONB DEFAULT '[]',
    notificaciones_activadas BOOLEAN DEFAULT TRUE
);

-- Médico
CREATE TABLE medico (
    id_usuario UUID PRIMARY KEY REFERENCES usuario(id_usuario) ON DELETE CASCADE,
    especialidad VARCHAR(100),
    centro_medico VARCHAR(150),
    pacientes_asignados JSONB DEFAULT '[]',
    notificaciones_activadas BOOLEAN DEFAULT TRUE,
    firma_digital TEXT
);

-- Paciente
CREATE TABLE paciente (
    id_usuario UUID PRIMARY KEY REFERENCES usuario(id_usuario) ON DELETE CASCADE,
    diagnostico_inicial VARCHAR(150),
    fecha_ingreso DATE DEFAULT CURRENT_DATE,
    id_cuidador UUID REFERENCES cuidador(id_usuario) ON DELETE SET NULL,
    id_medico UUID REFERENCES medico(id_usuario) ON DELETE SET NULL,
    observaciones_clinicas TEXT,
    foto_referencia TEXT
);

-- Índices
CREATE INDEX idx_usuario_email ON usuario(email);
CREATE INDEX idx_paciente_medico ON paciente(id_medico);
CREATE INDEX idx_paciente_cuidador ON paciente(id_cuidador);
