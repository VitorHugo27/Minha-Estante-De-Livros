# Publica a Minha Estante neste computador.
#
#   1. Para a versão publicada, se ela estiver rodando.
#   2. Instala os pacotes do Angular que faltarem e gera o site em modo de produção.
#   3. Publica a Api .NET em %LOCALAPPDATA%\Programs\MinhaEstante.
#   4. Coloca o site dentro da Api, na pasta wwwroot.
#   5. Cria a tarefa que inicia o app quando você entra no Windows e um atalho na área de trabalho.
#
# Para publicar de novo depois de mudar o código, rode na pasta do projeto:
#   powershell -ExecutionPolicy Bypass -File .\publicar.ps1

$ErrorActionPreference = 'Stop'

$raiz     = $PSScriptRoot
$destino  = Join-Path $env:LOCALAPPDATA 'Programs\MinhaEstante'
$tarefa   = 'Minha Estante'
$endereco = 'http://localhost:5080'

Write-Host '1/5 Parando a versão publicada, se estiver rodando...' -ForegroundColor Cyan
if (Get-ScheduledTask -TaskName $tarefa -ErrorAction SilentlyContinue) {
    Stop-ScheduledTask -TaskName $tarefa
}
Get-Process -Name 'MinhaEstante.Api' -ErrorAction SilentlyContinue |
    Where-Object { $_.Path -and $_.Path.StartsWith($destino, [StringComparison]::OrdinalIgnoreCase) } |
    Stop-Process -Force
Start-Sleep -Seconds 1

Write-Host '2/5 Gerando o site (Angular)...' -ForegroundColor Cyan
Push-Location (Join-Path $raiz 'frontend')
try {
    # Instala o que está no package.json e ainda não está na pasta node_modules.
    npm install --no-audit --no-fund
    if ($LASTEXITCODE -ne 0) { throw 'A instalação dos pacotes do Angular falhou. Veja as mensagens acima.' }
    npm run build
    if ($LASTEXITCODE -ne 0) { throw 'O build do Angular falhou. Veja as mensagens acima.' }
}
finally {
    Pop-Location
}

Write-Host '3/5 Publicando a Api (.NET)...' -ForegroundColor Cyan
$projetoApi = Join-Path $raiz 'backend\src\MinhaEstante.Api\MinhaEstante.Api.csproj'
dotnet publish $projetoApi -c Release -o $destino -p:SemJanela=true
if ($LASTEXITCODE -ne 0) { throw 'A publicação da Api falhou. Veja as mensagens acima.' }

Write-Host '4/5 Colocando o site dentro da Api...' -ForegroundColor Cyan
$wwwroot = Join-Path $destino 'wwwroot'
if (Test-Path $wwwroot) { Remove-Item $wwwroot -Recurse -Force }
Copy-Item (Join-Path $raiz 'frontend\dist\minha-estante\browser') $wwwroot -Recurse

Write-Host '5/5 Configurando a inicialização com o Windows...' -ForegroundColor Cyan
# O caminho do programa vai entre aspas porque pode ter espaços (C:\Users\Seu Nome\...).
# A pasta de trabalho fica sem aspas: a tarefa agendada não aceita aspas nesse campo.
$programa = Join-Path $destino 'MinhaEstante.Api.exe'
$parametros = @{
    TaskName    = $tarefa
    Description = "Inicia a Minha Estante em $endereco"
    Action      = New-ScheduledTaskAction -Execute "`"$programa`"" -WorkingDirectory $destino
    Trigger     = New-ScheduledTaskTrigger -AtLogOn -User "$env:USERDOMAIN\$env:USERNAME"
    Settings    = New-ScheduledTaskSettingsSet -ExecutionTimeLimit ([TimeSpan]::Zero) -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries
    Force       = $true
}
try {
    Register-ScheduledTask @parametros | Out-Null
}
catch {
    throw "Não foi possível criar a tarefa de inicialização. Abra o PowerShell como administrador e rode o script de novo. Detalhe: $($_.Exception.Message)"
}
Start-ScheduledTask -TaskName $tarefa

# O atalho abre o site no navegador e usa o ícone da Minha Estante.
$atalho = Join-Path ([Environment]::GetFolderPath('Desktop')) 'Minha Estante.url'
$icone  = Join-Path $wwwroot 'favicon.ico'
Set-Content -Path $atalho -Value "[InternetShortcut]`r`nURL=$endereco`r`nIconFile=$icone`r`nIconIndex=0" -Encoding ASCII

Write-Host ''
Write-Host "Pronto! A Minha Estante está rodando em $endereco" -ForegroundColor Green
Write-Host 'Ela inicia sozinha quando você entra no Windows, e há um atalho na área de trabalho.'
Write-Host 'Para instalar como app, abra o endereço no Edge ou no Chrome e clique em Instalar, na barra de endereço.'
