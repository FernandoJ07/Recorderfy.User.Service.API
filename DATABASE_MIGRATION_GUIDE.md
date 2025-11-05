# Guía de Migración de Base de Datos - Recorderfy User Service

## Problemas Corregidos

### 1. **Herencia Incorrecta**
- **Antes**: `Medico` y `Paciente` heredaban de `Usuario` (herencia en código)
- **Después**: Relación 1:1 mediante FK `IdUsuario` (correcto para el modelo de BD)

### 2. **Claves Primarias Faltantes**
- Se agregaron PKs a `Cuidador`, `Medico` y `Paciente` usando `IdUsuario`

### 3. **Navegaciones Corregidas**
- `Paciente.Cuidador` y `Paciente.Medico` (antes: `IdCuidadorNavigation`)
- Relaciones bidireccionales correctamente configuradas

### 4. **Configuración de DbContext**
- Relaciones 1:1 entre Usuario-Cuidador, Usuario-Medico, Usuario-Paciente
- Configuración CASCADE en eliminación para mantener integridad

## Comando de Scaffold Mejorado

### Desde el proyecto DAL:
```bash
cd Recorderfy.User.Service.DAL

dotnet ef dbcontext scaffold "Host=localhost;Port=5432;Database=RecorderfyUser;Username=postgres;Password=admin;" Npgsql.EntityFrameworkCore.PostgreSQL --startup-project "..\Recorderfy.User.Service.API" --output-dir "..\Recorderfy.User.Service.Model\Entities" --context-dir "Data" --context "ApplicationDbContext" --force --no-onconfiguring
```

### Parámetros Importantes:
- `--context "ApplicationDbContext"`: Nombre específico (evita conflictos con "DbContext")
- `--no-onconfiguring`: Evita generar `OnConfiguring` (usamos inyección de dependencias)
- `--force`: Sobrescribe archivos existentes

## Verificación Post-Scaffold

Después de ejecutar el scaffold, **SIEMPRE** verificar y ajustar:

1. **Entidades con relación 1:1 a Usuario**:
   - Agregar `public Guid IdUsuario { get; set; }` como PK
   - Agregar navegación: `public virtual Usuario Usuario { get; set; } = null!;`
   - Remover herencia de `Usuario` si la genera

2. **ApplicationDbContext**:
   - Configurar `HasKey()` para Cuidador, Medico, Paciente
   - Configurar relaciones 1:1 con `HasOne().WithOne()`
   - Usar `HasForeignKey<T>()` para especificar la FK

3. **Navegaciones en Usuario**:
   ```csharp
   public virtual Cuidador? Cuidador { get; set; }
   public virtual Medico? Medico { get; set; }
   public virtual Paciente? Paciente { get; set; }
   ```

## Script SQL de Verificación

Ejecutar en PostgreSQL para validar la estructura:

```sql
-- Verificar tablas principales
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public' 
  AND table_type = 'BASE TABLE'
ORDER BY table_name;

-- Verificar PKs
SELECT tc.table_name, kcu.column_name
FROM information_schema.table_constraints tc
JOIN information_schema.key_column_usage kcu 
  ON tc.constraint_name = kcu.constraint_name
WHERE tc.constraint_type = 'PRIMARY KEY'
  AND tc.table_schema = 'public'
ORDER BY tc.table_name;

-- Verificar FKs
SELECT 
    tc.table_name, 
    kcu.column_name, 
    ccu.table_name AS foreign_table_name,
    ccu.column_name AS foreign_column_name
FROM information_schema.table_constraints AS tc 
JOIN information_schema.key_column_usage AS kcu
  ON tc.constraint_name = kcu.constraint_name
JOIN information_schema.constraint_column_usage AS ccu
  ON ccu.constraint_name = tc.constraint_name
WHERE tc.constraint_type = 'FOREIGN KEY'
  AND tc.table_schema = 'public'
ORDER BY tc.table_name;
```

## Problemas Comunes y Soluciones

### Error: "No key was defined"
**Causa**: Falta la configuración de `HasKey()` en el DbContext  
**Solución**: Agregar `.HasKey(e => e.IdUsuario)` en las entidades especializadas

### Error: "Circular dependency"
**Causa**: Navegaciones bidireccionales mal configuradas  
**Solución**: Usar `HasOne().WithOne()` correctamente con `HasForeignKey<T>()`

### Error: "Multiple cascade paths"
**Causa**: SQL Server no permite múltiples cascadas  
**Solución**: Usar `.OnDelete(DeleteBehavior.SetNull)` o `Restrict` donde sea apropiado

### Scaffold sobrescribe configuraciones manuales
**Causa**: EF Core regenera todo desde la BD  
**Solución**: Usar clases parciales (`partial class`) para extensiones personalizadas

## Workflow Recomendado

1. **Desarrollo database-first**:
   - Modificar BD en PostgreSQL
   - Ejecutar scaffold
   - Revisar y ajustar entidades generadas
   - Revisar y ajustar ApplicationDbContext
   - Probar migraciones

2. **Testing**:
   ```bash
   dotnet build
   dotnet test
   ```

3. **Validar en runtime**:
   ```bash
   cd Recorderfy.User.Service.API
   dotnet run
   ```

## Notas Adicionales

- **ConnectionString**: Nunca commitear passwords reales en `appsettings.json`
- **Usar User Secrets** para desarrollo local:
  ```bash
  dotnet user-secrets init
  dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=RecorderfyUser;Username=postgres;Password=admin;"
  ```

- **Para producción**: Usar variables de entorno o Azure Key Vault
