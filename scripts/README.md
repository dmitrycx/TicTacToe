# Scripts Directory

This directory contains organized utility scripts for development, testing, and deployment.

## 📁 **Script Organization**

| Script | Purpose | Usage |
|--------|---------|-------|
| `test-containers-local.sh` | Local container testing | `./scripts/testing/test-containers-local.sh` |
| `test-containers-ci-local.sh` | CI container testing | `./scripts/testing/test-containers-ci-local.sh` |
| `test-frontend-local.sh` | Frontend testing | `./scripts/testing/test-frontend-local.sh` |
| `build-local-containers.sh` | Build local containers | `./scripts/development/build-local-containers.sh` |

## 🚀 **Quick Commands**

### **Testing Scripts**
```bash
# Test containers locally
./scripts/testing/test-containers-local.sh

# Test containers for CI
./scripts/testing/test-containers-ci-local.sh

# Test frontend
./scripts/testing/test-frontend-local.sh
```

### **Development Scripts**
```bash
# Build local containers
./scripts/development/build-local-containers.sh
```

## 📋 **Script Standards**

- **Clear naming**: Scripts are named descriptively
- **Error handling**: All scripts include proper error checking
- **Documentation**: Each script has inline comments
- **Portability**: Scripts work on macOS, Linux, and Windows (WSL)

## 🔧 **Script Categories**

### **Testing Scripts** (`testing/`)
- **Container Testing**: Docker container validation
- **Frontend Testing**: Next.js and Playwright testing
- **Integration Testing**: End-to-end validation

### **Development Scripts** (`development/`)
- **Container Building**: Local Docker image creation
- **Environment Setup**: Development environment configuration

### **Deployment Scripts** (`deployment/`)
- **Production Deployment**: Live environment deployment
- **Staging Deployment**: Pre-production validation

## 📚 **Related Documentation**

- **[Testing Guide](../docs/testing/README.md)** - Complete testing documentation
- **[Development Guide](../docs/development/README.md)** - Development workflow
- **[Deployment Guide](../docs/deployment/README.md)** - Production deployment 