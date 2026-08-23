# BootcampCLT2026 — API de Cuentas

API REST desarrollada en **.NET 10** siguiendo un enfoque de **Clean Architecture** con **CQRS** (MediatR), pensada para gestionar cuentas bancarias. Incluye persistencia en PostgreSQL, logging centralizado con Seq, y está preparada para desplegarse en Kubernetes vía Helm, con un pipeline de CI/CD automatizado en GitHub Actions.

## Tabla de contenidos

- [Arquitectura y stack técnico](#arquitectura-y-stack-técnico)
- [Estructura del proyecto](#estructura-del-proyecto)
- [Requisitos previos](#requisitos-previos)
- [Opción A — Levantar todo en Kubernetes (recomendado)](#opción-a--levantar-todo-en-kubernetes-recomendado)
- [Opción B — Correr la API localmente sin Kubernetes](#opción-b--correr-la-api-localmente-sin-kubernetes)
- [Variables de configuración](#variables-de-configuración)
- [Endpoints principales](#endpoints-principales)
- [Health checks](#health-checks)
- [Observabilidad con Seq](#observabilidad-con-seq)
- [CI/CD](#cicd)
- [Troubleshooting](#troubleshooting)

## Arquitectura y stack técnico

| Componente | Tecnología |
|---|---|
| Framework | .NET 10, Minimal APIs |
| Patrón | Clean Architecture + CQRS (MediatR) |
| Validación | FluentValidation |
| Base de datos | PostgreSQL 16 (Npgsql + EF Core, snake_case naming) |
| Logging | Serilog → Consola + Seq |
| Documentación de API | Scalar (OpenAPI), disponible solo en entorno `Development` |
| Contenedores | Docker |
| Orquestación | Kubernetes (probado en Minikube, driver Docker) |
| Empaquetado de despliegue | Helm |
| CI/CD | GitHub Actions (build/test/push + deploy automático) |

## Estructura del proyecto

```
BootcampCLT2026/
├── Application/        # Casos de uso (CQRS: Commands, Queries, Validators)
├── Domain/              # Entidades y contratos de repositorio
├── Infraestructure/     # EF Core, repositorios, DbContext
├── Endpoints/           # Definición de endpoints Minimal API
├── Middleware/          # Manejo de excepciones de validación
├── database/            # Scripts SQL de creación de base y seed
├── K8s/                 # Manifiestos de Kubernetes para PostgreSQL y Seq
├── helm/cuenta-api/     # Chart de Helm para desplegar la API
├── .github/workflows/   # Pipeline de CI/CD
├── Dockerfile
└── Program.cs
```

## Requisitos previos

Instalar en la máquina donde se va a desplegar:

- [.NET SDK 10.0](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (con WSL2 habilitado en Windows)
- [Minikube](https://minikube.sigs.k8s.io/docs/start/)
- [kubectl](https://kubernetes.io/docs/tasks/tools/)
- [Helm 3](https://helm.sh/docs/intro/install/)

> **Nota para Windows Home:** Hyper-V no está disponible en esta edición. Usar el driver **Docker** de Minikube (`minikube start --driver=docker`), no Hyper-V.

### 1. Clonar el repositorio

```bash
git clone https://github.com/alvaroguillen/BootcampCLT2026.git
cd BootcampCLT2026
```

### 2. Iniciar Minikube

```bash
minikube start --driver=docker
```

### 3. Desplegar PostgreSQL y Seq

Estos manifiestos crean el namespace `cuentas`, la base de datos, el volumen persistente y el servidor de logs:

```bash
kubectl apply -f K8s/00-namespace.yaml
kubectl apply -f K8s
```

Esperar a que ambos estén listos:

```bash
kubectl rollout status deployment/postgres --namespace cuentas --timeout=120s
kubectl rollout status deployment/seq --namespace cuentas --timeout=120s
```

### 4. Crear el esquema de base de datos y los datos de ejemplo

```bash
kubectl exec -i -n cuentas deployment/postgres -- psql -U postgres -d accountsdb < database/02_create_table_and_seed.sql
```

> La base de datos `accountsdb` se crea automáticamente al inicializar el contenedor de Postgres (vía la variable `POSTGRES_DB`); el archivo `database/01_create_database.sql` queda solo como referencia por si se conecta contra un Postgres externo ya existente.

### 5. Desplegar la API con Helm

```bash
helm upgrade --install api-cuenta ./helm/cuenta-api \
  --namespace cuentas \
  --create-namespace \
  --wait --timeout 3m
```

Este comando construye y despliega los recursos definidos en `helm/cuenta-api/values.yaml`, incluyendo el Deployment de la API (que espera a que Postgres esté listo mediante un init container), su Service, y las variables de conexión.

> Por defecto, `values.yaml` apunta a la imagen publicada en Docker Hub (`leticiaalvaro22/api-cuentas:latest`). Para usar una imagen construida localmente en su lugar, ver la sección de build manual más abajo.

### 6. Verificar que todo esté corriendo

```bash
kubectl get pods -n cuentas
kubectl get svc -n cuentas
```

Deberían verse tres pods en estado `1/1 Running`: `postgres`, `seq` y `api-cuenta`.

### 7. Acceder a la API

Con el driver Docker en Windows/Mac, el método más confiable es `port-forward`:

```bash
kubectl port-forward -n cuentas svc/api-cuenta 8080:8080
```

Y abrir en el navegador:

```
http://localhost:8080/scalar
```

Ahí se puede explorar y probar todos los endpoints de forma interactiva.

## Endpoints principales

Ruta base: `/v1/api/cuenta`

| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/v1/api/cuenta` | Lista todas las cuentas |
| `GET` | `/v1/api/cuenta/{id}` | Obtiene una cuenta por ID |
| `POST` | `/v1/api/cuenta` | Crea una nueva cuenta |
| `PUT` | `/v1/api/cuenta/{id}` | Modifica una cuenta existente |
| `DELETE` | `/v1/api/cuenta/{id}` | Elimina una cuenta |

La documentación interactiva completa está disponible en `/scalar` cuando la app corre en modo `Development`.

## Observabilidad con Seq

```bash
kubectl port-forward -n cuentas svc/seq 5341:5341
```

Abrir `http://localhost:5341` para ver los logs estructurados de la API en tiempo real.

## CI/CD

El pipeline (`.github/workflows/ci.yml`) se dispara en cada push a `main` y consta de dos jobs:

1. **`ci`** (runner de GitHub en la nube): restaura, compila, corre tests, valida el chart de Helm (`helm lint`), y construye/publica la imagen Docker en Docker Hub.
2. **`cd`** (runner **self-hosted**, debe correr en una máquina con Minikube activo): se asegura de que Minikube esté levantado, aplica los manifiestos de PostgreSQL/Seq, ejecuta el script de seed, y despliega la API actualizada con `helm upgrade --install`.

### Secrets requeridos en el repositorio de GitHub

| Secret | Descripción |
|---|---|
| `DOCKERHUB_USERNAME` | Usuario de Docker Hub |
| `DOCKERHUB_TOKEN` | Access token de Docker Hub (no la contraseña) |

### Requisito del runner `cd`

Este job necesita un runner **self-hosted** registrado en el repositorio, corriendo en una máquina Windows con Minikube, Docker, kubectl y Helm instalados — GitHub no tiene acceso a un clúster local desde sus runners en la nube.

## Troubleshooting

**`Hyper-V PowerShell Module is not available`** al iniciar Minikube en Windows Home → usar `minikube start --driver=docker` en lugar de Hyper-V.

**`kubeconfig: Misconfigured`** al correr `minikube status` → correr `minikube update-context` para resincronizar el contexto de kubectl, o `minikube stop && minikube start` si persiste.

**Pod en `CrashLoopBackOff`** → revisar los logs con `kubectl logs -n cuentas <pod> --previous`; las causas más comunes son un error en la cadena de conexión a Postgres o una probe apuntando a una ruta incorrecta.

**`role "postgres" does not exist"` al conectarse a Postgres** → generalmente indica que el volumen (`PVC`) ya fue inicializado con credenciales distintas a las del `Secret` actual. Solución: eliminar el deployment y el PVC de Postgres y volver a aplicarlos para forzar una reinicialización limpia.

```bash
kubectl delete deployment postgres -n cuentas
kubectl delete pvc postgres-pvc -n cuentas
kubectl apply -f K8s/02-postgres-pvc.yaml
kubectl apply -f K8s/03-postgres-deployment.yaml
```

**No se puede acceder vía `minikube service --url`** → con el driver Docker en Windows, este método requiere mantener la terminal abierta durante toda la sesión. Se recomienda usar `kubectl port-forward` como alternativa más estable.

## Licencia

Proyecto desarrollado con fines educativos como parte del Bootcamp CLT 2026.
