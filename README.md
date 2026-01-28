# 📄 ScrapingWashes - Preenchedor de Citações Automático

Este projeto é um bot automatizado que preenche citações ausentes em uma planilha do Google Sheets. Ele lê títulos de artigos, busca no Google Scholar (via ScraperAPI) e atualiza a planilha automaticamente.

---

## 🚀 Guia de Início Rápido (Passo a Passo)

Este guia cobre desde a instalação até a execução e validação dos dados.

### 📋 Pré-requisitos

1.  **Windows, Mac ou Linux**.
2.  **.NET 8.0 SDK** instalado ([Download aqui](https://dotnet.microsoft.com/pt-br/download/dotnet/8.0)).
3.  **Conta Google** (para acessar a planilha).
4.  **Chave da ScraperAPI** (para evitar bloqueios do Google). Crie uma conta em [ScraperAPI](https://www.scraperapi.com/) e pegue sua chave.

---

### Passo 1: Clonar e Compilar o Projeto

1.  Abra seu terminal (PowerShell ou CMD no Windows).
2.  Clone este repositório:
    ```bash
    git clone https://github.com/gesid/ScrapingWashes.git
    ```
3.  Entre na pasta do projeto:
    ```bash
    cd ScrapingWashes
    ```
4.  Baixe as dependências e compile o código:
    ```bash
    dotnet restore
    dotnet build
    ```

---

### Passo 2: Configurar Credenciais do Google (OAuth)

Para que o bot acesse sua planilha, você precisa criar credenciais no Google Cloud.

1.  Acesse o [Google Cloud Console](https://console.cloud.google.com/).
2.  Crie um **Novo Projeto** (ex: "ScrapingWashes").
3.  Vá em **APIs e Serviços > Biblioteca** e ative a **Google Sheets API**.
4.  Vá em **APIs e Serviços > Credenciais** e clique em **Criar Credenciais > ID do cliente OAuth**.
5.  Configure a tela de permissão (se solicitado):
    *   **Tipo de usuário**: Externo.
    *   **Escopos**: Pode pular.
    *   **Usuários de teste**: Adicione **seu e-mail gmail** (importante!).
6.  Em "Tipo de Aplicativo", escolha **App para computador** (Desktop app).
7.  Baixe o arquivo JSON, renomeie para `credentials.json` e **coloque-o dentro da pasta principal do código**:
    *   Caminho: `ScrapingWashes\ScrapingWashes\credentials.json`

---

### Passo 3: Configurar o Projeto (appsettings.json)

1.  Na pasta `ScrapingWashes\ScrapingWashes`, localize o arquivo `appsettings.template.json`.
2.  Renomeie este arquivo para `appsettings.json`.
3.  Abra o arquivo e preencha as informações:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    },
    "SpreadsheetId": "SEU_ID_DA_PLANILHA_AQUI",
    "SheetName": "NOME_DA_ABA_AQUI"
  },
  "Variables": {
    "ScraperApiKey": "SUA_CHAVE_SCRAPERAPI_AQUI"
  },
  "AllowedHosts": "*"
}
```

*   **SpreadsheetId**: É o código longo na URL da sua planilha.
    *   Exemplo: Na URL `https://docs.google.com/spreadsheets/d/1ogR7v57DApDMSz2lZfG5ajf6s7ekIer7BPjczNZaVSo/edit`, o ID é `1ogR7v57DApDMSz2lZfG5ajf6s7ekIer7BPjczNZaVSo`.
*   **SheetName**: O nome da aba na barra inferior da planilha (ex: "Sheet1", "Página1", "Dados").
*   **ScraperApiKey**: Sua chave obtida no site da ScraperAPI.

---

### Passo 4: Configurar a Planilha

Certifique-se de que sua planilha segue este padrão:

*   **Coluna C**: Deve conter os **Títulos dos Artigos** (o bot usará isso para buscar).
*   **Coluna O**: Onde as **Citações** serão salvas. 

---

### Passo 5: Executar e Logar

1.  No terminal, execute o bot:
    ```bash
    dotnet run --project ScrapingWashes
    ```
    *(Ou entre na pasta ScrapingWashes e rode apenas `dotnet run`)*

2.  **Apenas na primeira vez**:
    *   Uma janela do navegador abrirá.
    *   Faça login com sua conta Google (a mesma que tem acesso à planilha).
    *   Permita o acesso ("Continuar" mesmo se aparecer aviso de app não verificado, pois é seu próprio app).
    *   O bot salvará um token e não pedirá login nas próximas vezes.

---

### Passo 6: Validação Manual (Importante! ⚠️)

Após o bot terminar ou processar algumas linhas, faça uma verificação manual para garantir a qualidade dos dados:

1.  Abra sua planilha no Google Sheets.
2.  Vá até a **Coluna O**.
3.  Verifique se as citações preenchidas correspondem aos artigos da **Coluna C**.
4.  O bot pode ter marcado algumas células com `#`. Isso significa que ele buscou mas não encontrou citações confiáveis ou o artigo tinha 0 citações.
5.  Confirme se o formato está correto (ex: Autor, Ano - Título).

**Dica**: Monitore o terminal enquanto o bot roda, ele mostrará logs detalhados do que está fazendo (ex: "Row 5: Fetching citation...").

---

## 🛠 Solução de Problemas

*   **Erro "credentials.json not found"**: Verifique se o arquivo está na pasta correta (`ScrapingWashes\ScrapingWashes`).
*   **Erro de Permissão (403)**: Verifique se seu email está adicionado como "Usuário de Teste" no Google Cloud Console.
*   **Nenhuma citação encontrada**: Verifique se sua chave ScraperAPI é válida e tem créditos.
