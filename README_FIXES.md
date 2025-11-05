# Recorderfy User Service - Correcciones Aplicadas

## 📋 Resumen de Problemas Corregidos

### 1. ❌ Modelo de Herencia Incorrecto
**Problema**: Las entidades `Medico` y `Paciente` heredaban de `Usuario` usando herencia de clases en C#, pero en la base de datos están en tablas separadas (modelo Table-Per-Type mal implementado).

**Solución**: 
- Se eliminó la herencia de `Usuario`
- Se establecieron relaciones 1:1 mediante Foreign Key `IdUsuario`
- Se agregó la propiedad `IdUsuario` como Primary Key en las entidades especializadas

**Archivos modificados**:
- `Recorderfy.User.Service.Model/Entities/Medico.cs`
- `Recorderfy.User.Service.Model/Entities/Paciente.cs`
- `Recorderfy.User.Service.Model/Entities/Cuidador.cs`

### 2. ❌ Claves Primarias Faltantes
**Problema**: Las tablas `cuidador`, `medico` y `paciente` no tenían Primary Keys definidas correctamente.

**Solución**:
- Se agregó `public Guid IdUsuario { get; set; }` como PK en cada entidad
- Se configuró correctamente en el `ApplicationDbContext` con `HasKey()`

### 3. ❌ Navegaciones Incorrectas
**Problema**: Las navegaciones en `Paciente` usaban nombres inconsistentes (`IdCuidadorNavigation`, `IdMedicoNavigation`).

**Solución**:
- Renombradas a `Cuidador` y `Medico` (convención estándar de EF Core)
- Configuradas correctamente las relaciones bidireccionales

### 4. ❌ Configuración Incompleta del DbContext
**Problema**: Faltaba la configuración de relaciones 1:1 y las claves primarias.

**Solución**:
- Se agregaron configuraciones completas para cada entidad
- Se establecieron relaciones 1:1 con `HasOne().WithOne()`
- Se configuraron correctamente los `DELETE CASCADE` y `SET NULL`

## 🔧 Archivos Modificados

### Entidades (Model Layer)
```
Recorderfy.User.Service.Model/Entities/
├── Cuidador.cs      ✅ Agregado IdUsuario como PK, corregida navegación
├── Medico.cs        ✅ Eliminada herencia, agregado IdUsuario, corregida navegación
└── Paciente.cs      ✅ Eliminada herencia, agregado IdUsuario, corregidas navegaciones
```

### Capa de Datos (DAL Layer)
```
Recorderfy.User.Service.DAL/Data/
└── ApplicationDbContext.cs  ✅ Configuraciones completas de entidades y relaciones
```

### Archivos de Documentación Creados
```
Recorderfy/
├── DATABASE_MIGRATION_GUIDE.md  📖 Guía completa de migración
├── database_setup.sql            🗄️ Script de validación/creación de BD
└── README_FIXES.md               📄 Este archivo
```

## 🚀 Pasos para Aplicar las Correcciones

### 1. Validar la Base de Datos
Ejecuta el script SQL para asegurar que la estructura de BD es correcta:

```bash
psql -U postgres -d RecorderfyUser -f database_setup.sql
```

### 2. Verificar las Entidades
Las entidades ya fueron corregidas. Verifica que compilan sin errores:

```bash
cd Recorderfy.User.Service.DAL
dotnet build
```

### 3. (Opcional) Re-Scaffold desde la Base de Datos
Si necesitas regenerar las entidades desde la BD:

```bash
cd Recorderfy.User.Service.DAL

dotnet ef dbcontext scaffold "Host=localhost;Port=5432;Database=RecorderfyUser;Username=postgres;Password=admin;" Npgsql.EntityFrameworkCore.PostgreSQL --startup-project "..\Recorderfy.User.Service.API" --output-dir "..\Recorderfy.User.Service.Model\Entities" --context-dir "Data" --context "ApplicationDbContext" --force --no-onconfiguring
```

**⚠️ IMPORTANTE**: Después del scaffold, deberás aplicar manualmente las correcciones de nuevo (ver `DATABASE_MIGRATION_GUIDE.md`)

### 4. Ejecutar la Aplicación
```bash
cd ..\Recorderfy.User.Service.API
dotnet run
```

### 5. Probar la API
Accede a Swagger en: `https://localhost:5001/swagger`

## 📊 Estructura de Relaciones Corregida

```
┌─────────────────┐
│  TipoDocumento  │
│  (id_tipo_doc)  │
└────────┬────────┘
         │ 1
         │
         │ N
┌────────▼────────┐       1:1        ┌──────────────┐
│     Rol         │                   │   Cuidador   │
│   (id_rol)      │                   │ (id_usuario) │◄──┐
└────────┬────────┘                   └──────┬───────┘   │
         │ 1                                 │ 1         │
         │                                   │           │
         │ N                          1:N    │           │
┌────────▼────────┐       1:1        ┌──────▼───────┐   │
│    Usuario      ├─────────────────►│  Paciente    │   │
│  (id_usuario)   │                   │ (id_usuario) │───┘
└────────┬────────┘                   └──────┬───────┘
         │                                   │
         │ 1:1                        1:N    │
         │                                   │
         ▼                                   │
┌─────────────────┐                          │
│     Medico      │◄─────────────────────────┘
│  (id_usuario)   │
└─────────────────┘
```

## 🔍 Validación de Correcciones

### Verificar Estructura de Tablas
```sql
-- Ver tablas
\dt

-- Ver estructura de paciente
\d paciente

-- Ver constraints
SELECT 
    tc.table_name, 
    tc.constraint_name, 
    tc.constraint_type
FROM information_schema.table_constraints tc
WHERE tc.table_schema = 'public'
ORDER BY tc.table_name;
```

### Verificar Código C#
```bash
# Compilar todo el proyecto
dotnet build

# Ejecutar tests (si existen)
dotnet test

# Verificar errores de EF Core
cd Recorderfy.User.Service.API
dotnet ef dbcontext info --startup-project .
```

## 📚 Documentación Adicional

- **DATABASE_MIGRATION_GUIDE.md**: Guía completa de migración y scaffold
- **database_setup.sql**: Script SQL para crear/validar la estructura de BD
- **Swagger UI**: Documentación automática de la API en `/swagger`

## ⚠️ Advertencias Importantes

1. **No usar scaffold sin backup**: El comando `--force` sobrescribe archivos
2. **Configurar User Secrets**: No commitear passwords en `appsettings.json`
3. **Revisar siempre después del scaffold**: EF Core puede generar configuraciones subóptimas
4. **Testing antes de producción**: Validar todas las operaciones CRUD

## 🐛 Problemas Conocidos y Soluciones

### Error: "No key was defined for entity type 'Cuidador'"
**Solución**: Verificar que en `ApplicationDbContext.OnModelCreating` existe:
```csharp
modelBuilder.Entity<Cuidador>(entity =>
{
    entity.HasKey(e => e.IdUsuario).HasName("cuidador_pkey");
    // ...
});
```

### Error: "Cannot insert duplicate key in object 'dbo.usuario'"
**Solución**: Verificar que no estás intentando crear un Usuario y un Medico/Paciente/Cuidador con el mismo ID sin la relación correcta.

### Error: "Circular dependency detected"
**Solución**: Las relaciones 1:1 están correctamente configuradas con `HasForeignKey<T>()` para evitar ambigüedad.

## 📞 Contacto y Soporte

Para problemas o preguntas adicionales, revisar:
1. Los archivos de documentación en este repositorio
2. La documentación oficial de EF Core: https://docs.microsoft.com/ef/core/
3. PostgreSQL documentation: https://www.postgresql.org/docs/

---

**Fecha de corrección**: Octubre 29, 2025  
**Versión del framework**: .NET 8.0  
**Versión de EF Core**: 9.0.10  
**Base de datos**: PostgreSQL 12+
