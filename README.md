# CZURoomsApp

## Požadavky pro sestavení
* .NET Core 3
* GUI framework dle platformy

## Instalace
1. stáhněte si tenhle repozitář
```bash
git clone git@github.com:Tajnymag/CZURoomsApp.git
```

2. vlezte do složky s naklonovaným repozitářem a stáhněte si závislosti
```bash
cd CZURoomsApp && dotnet restore
```

3. zkompilujte aplikaci pro vaši platformu
```bash
# Windows
cd CZURoomsApp.Wpf && dotnet build

# Linux
cd CZURoomsApp.Gtk && dotnet build

# MacOS
cd CZURoomsApp.Mac && dotnet build
```
