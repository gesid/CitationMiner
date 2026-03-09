# CitationMiner

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
git clone https://github.com/gesid/CitationMiner.git
```

2. Navegue até o diretório do projeto:
```shell
cd CitationMiner
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

---

### Passo 2: Configurar Credenciais do Google (OAuth)

Para que o bot acesse sua planilha, você precisa criar credenciais no Google Cloud.

1.  Acesse o [Google Cloud Console](https://console.cloud.google.com/).
2.  Crie um **Novo Projeto** (ex: "CitationMiner").
3.  Vá em **APIs e Serviços > Biblioteca** e ative a **Google Sheets API**.
4.  Vá em **APIs e Serviços > Credenciais** e clique em **Criar Credenciais > ID do cliente OAuth**.
5.  Configure a tela de permissão (se solicitado):
    *   **Tipo de usuário**: Externo.
    *   **Escopos**: Pode pular.
    *   **Usuários de teste**: Adicione **seu e-mail gmail** (importante!).
6.  Em "Tipo de Aplicativo", escolha **App para computador** (Desktop app).
7.  Baixe o arquivo JSON, renomeie para `credentials.json` e **coloque-o dentro da pasta principal do código**:
    *   Caminho: `CitationMiner\CitationMiner\credentials.json`

---

### Passo 3: Configurar o Projeto (appsettings.json)

1.  Na pasta `CitationMiner\CitationMiner`, localize o arquivo `appsettings.template.json`.
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
    dotnet run --project CitationMiner
    ```
    *(Ou entre na pasta CitationMiner e rode apenas `dotnet run`)*

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

*   **Erro "credentials.json not found"**: Verifique se o arquivo está na pasta correta (`CitationMiner\CitationMiner`).
*   **Erro de Permissão (403)**: Verifique se seu email está adicionado como "Usuário de Teste" no Google Cloud Console.
*   **Nenhuma citação encontrada**: Verifique se sua chave ScraperAPI é válida e tem créditos.
