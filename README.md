# ScrapingWashes - Citation Filler

## Descrição
Esta é uma aplicação automatizada que preenche citações ausentes em uma planilha do Google Sheets, buscando informações no Google Scholar.

## O que a aplicação faz
- Lê títulos de artigos da **Coluna C** da planilha
- Verifica se a **Coluna O** (citações) está vazia
- Busca a citação no Google Scholar
- Preenche a citação no formato APA na **Coluna O**

## Pré-requisitos
- [.NET 8.0 SDK](https://dotnet.microsoft.com/pt-br/download)
- Conta Google com acesso à planilha
- Credenciais OAuth do Google Cloud (veja `SETUP_OAUTH.md`)

## Instalação

1. Clone o repositório
```shell
git clone https://github.com/gesid/ScrapingWashes.git
```

2. Navegue até o diretório do projeto:
```shell
cd ScrapingWashes
```

3. Configure as credenciais OAuth (veja `SETUP_OAUTH.md` para instruções detalhadas)

4. Restaure as dependências do projeto:
```shell
dotnet restore
```

5. Compile o projeto:
```shell
dotnet build
```

## Configuração

### 1. Credenciais OAuth
Siga as instruções em `SETUP_OAUTH.md` para:
- Criar projeto no Google Cloud
- Habilitar Google Sheets API
- Baixar `credentials.json`
- Colocar o arquivo em `ScrapingWashes\ScrapingWashes\`

### 2. Nome da Aba
Atualize o nome da aba no arquivo `Program.cs`:
```csharp
var sheetName = "Sheet1"; // Altere para o nome da sua aba
```

### 3. ID da Planilha
Já configurado: `1ogR7v57DApDMSz2lZfG5ajf6s7ekIer7BPjczNZaVSo`

## Execução

```shell
dotnet run
```

### Primeira Execução
- Um navegador abrirá automaticamente
- Faça login com sua conta Google
- Conceda as permissões necessárias
- O token será salvo para execuções futuras

### Execuções Subsequentes
- Apenas execute `dotnet run`
- Não será necessário fazer login novamente

## Estrutura da Planilha
- **Coluna C**: Títulos dos artigos (usado para busca)
- **Coluna O**: Citações (será preenchido se vazio)
- **Linha 1**: Cabeçalho (ignorado)

## Funcionalidades
- ✅ Pula linhas onde já existe citação
- ✅ Processa em lotes de 10 para evitar bloqueio
- ✅ Adiciona delay de 2 segundos entre lotes
- ✅ Mostra progresso no console
- ✅ Exibe quantas citações foram preenchidas

## Documentação Adicional
- `QUICKSTART.md` - Guia rápido de uso
- `SETUP_OAUTH.md` - Instruções detalhadas de configuração OAuth

## Solução de Problemas

### "credentials.json not found"
Certifique-se de que o arquivo está em `ScrapingWashes\ScrapingWashes\`

### "Spreadsheet not found"
- Verifique se está logado com a conta correta
- Confirme o ID da planilha no `Program.cs`

### Nenhuma citação encontrada
- Verifique o cookie do Google Scholar em `appsettings.json`
- O cookie pode ter expirado

## Tecnologias Utilizadas
- .NET 8.0
- Google Sheets API v4
- HtmlAgilityPack (para scraping)
- OAuth 2.0 (autenticação)
