# Definir nome da imagem
$ImageTag = "prd"
$ImageName = "learning-sec-api:$($ImageTag)"
$ContainerName = "learning-sec-api-$($ImageTag)"

# Construir a imagem Docker
Write-Host "Construindo a imagem Docker..."
docker build -f "C:\Development\sandbox\Learn\SecurityDomain\Learn.Security.Api\Dockerfile" --force-rm  -t $ImageName --build-arg "BUILD_CONFIGURATION=Release" "C:\Development\sandbox\Learn"

# Verificar se a construção foi bem-sucedida
if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro ao construir a imagem Docker." -ForegroundColor Red
    exit $LASTEXITCODE
}

## $existingContainer = docker ps -a --filter "name=$ContainerName" --format "{{.ID}}"
## 
## if ($existingContainer) {
##     Write-Host "Parando e removendo o container existente..."
##     docker stop $ContainerName
##     docker rm $ContainerName
## }
## 
## # Executar o contêiner
## Write-Host "Executando o container..."
## docker run -dt -p 5004:8080 -p 5005:8081 -e ASPNETCORE_ENVIRONMENT="Production" -e ASPNETCORE_HTTP_PORTS:8080 -e ASPNETCORE_HTTPS_PORTS=8081 -e ASPNETCORE_Kestrel__Certificates__Default__Password="hugocorreia77" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx -v $env:USERPROFILE\.aspnet\https:/https/ --name $ContainerName --network learning-network $ImageName 
## 
## # Verificar se o contêiner iniciou corretamente
## if ($LASTEXITCODE -ne 0) {
##     Write-Host "Erro ao executar o contêiner." -ForegroundColor Red
##     exit $LASTEXITCODE
## }
## 
## Write-Host "Container iniciado com sucesso!" -ForegroundColor Green
## 