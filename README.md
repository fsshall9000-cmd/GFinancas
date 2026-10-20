# GFinanças

Aplicação web para organizar as finanças pessoais: registrar receitas e despesas, acompanhar saldos de contas, planejar orçamentos e controlar aplicações financeiras e a evolução do patrimônio.

O projeto combina uma API REST em **ASP.NET Core / .NET 10**, persistência em **MySQL com Entity Framework Core** e uma interface em **Angular 19**, com gráficos e autenticação JWT.

## Índice

- [Funcionalidades](#funcionalidades)
- [Tecnologias](#tecnologias)
- [Arquitetura e estrutura](#arquitetura-e-estrutura)
- [Pré-requisitos](#pré-requisitos)
- [Instalação e execução local](#instalação-e-execução-local)
- [Configurações](#configurações)
- [Primeiro acesso](#primeiro-acesso)
- [API REST](#api-rest)
- [Autenticação e recuperação de senha](#autenticação-e-recuperação-de-senha)
- [Testes e compilação](#testes-e-compilação)
- [Publicação da aplicação](#publicação-da-aplicação)
- [Solução de problemas](#solução-de-problemas)
- [Contribuição](#contribuição)

## Funcionalidades

| Área | Recursos |
| --- | --- |
| Usuários | Cadastro, login, consulta do próprio perfil e recuperação de senha por e-mail. |
| Contas | Cadastro, edição, exclusão, consulta de contas e acompanhamento de saldo. |
| Categorias | Organização de lançamentos com cadastro, edição e exclusão de categorias. |
| Transações | Registro de receitas e despesas, edição, exclusão, filtros por período, tipo, conta e categoria, além de consulta paginada. |
| Orçamentos | Planejamento de limites por categoria e período, com acompanhamento no painel. |
| Aplicações financeiras | Cadastro, edição, desativação, atualização de valor e histórico de valores. |
| Movimentações de investimentos | Registro de aportes e resgates, com consulta do histórico por aplicação. |
| Dashboard | Visões sintética, financeira, patrimonial e de orçamentos; resumo financeiro, despesas por categoria, histórico mensal, investimentos e evolução patrimonial. |

As modalidades de aplicação disponíveis são Tesouro Direto, CDB, LCI, LCA, fundo de investimento, ação, ETF e outros. O histórico diferencia movimentações de atualizações de valor de mercado. Os valores de mercado são atualizados pelas operações da aplicação; o repositório não inclui integração automática com provedores de cotações.

## Tecnologias

| Camada | Tecnologias usadas no repositório |
| --- | --- |
| Backend | C#, .NET 10, ASP.NET Core, injeção de dependências. |
| Persistência | Entity Framework Core 10, provedor `MySql.EntityFrameworkCore` 10 e migrations. |
| Autenticação | JWT Bearer, assinatura HMAC-SHA256 e serviço de hash de senha. |
| Recuperação de senha | ASP.NET Core Data Protection, SMTP e limitação de requisições. |
| Documentação da API | OpenAPI e Scalar em ambiente de desenvolvimento. |
| Frontend | Angular 19.2, TypeScript 5.7, RxJS 7.8 e Angular CDK. |
| Interface e gráficos | Bootstrap 5.3, Chart.js 4 e ng2-charts 7. |
| Testes | xUnit, Moq e Coverlet no backend; Jasmine e Karma no frontend. |

As versões exatas das dependências estão nos arquivos `.csproj`, no `package.json` e no `package-lock.json`.

## Arquitetura e estrutura

O backend organiza as responsabilidades em camadas, seguindo a proposta de Clean Architecture:

- **Domain:** entidades e enumerações do negócio.
- **Application:** DTOs, regras de negócio, serviços, contratos de repositórios e exceções.
- **Infrastructure:** acesso ao MySQL, contexto do EF Core, migrations, repositórios, unidade de trabalho e emissão de JWT.
- **API:** controllers HTTP, configuração da aplicação, autenticação, tratamento de exceções e envio de recuperação de senha.

A API recebe as requisições, encaminha as operações aos serviços da aplicação e utiliza a infraestrutura para persistência. O Angular consome os endpoints por serviços HTTP, acrescentando o token por um interceptor e protegendo a navegação por um guard.

```text
GFinancas/
├── GFinancas.slnx                     # Solução .NET
├── src/
│   ├── GFinancas.API/                 # API REST e configuração do host
│   ├── GFinancas.Application/         # Casos de uso, contratos e DTOs
│   ├── GFinancas.Domain/              # Entidades e enumerações
│   └── GFinancas.Infrastructure/      # Persistência, migrations e JWT
├── frontend/gfinancas-web/
│   ├── public/                       # Arquivos estáticos
│   └── src/
│       ├── app/core/                 # Serviços, guard e interceptor
│       ├── app/features/             # Login, dashboard e cadastros
│       ├── app/pages/aplicacoes/     # Aplicações e movimentações
│       ├── app/models/               # Modelos da API
│       └── enviroments/              # URL da API (grafia atual do diretório)
├── tests/
│   ├── GFinancas.Tests/              # Testes xUnit
│   └── GFinancas.Auth.Checks/         # Verificações executáveis de recuperação
└── docs/autenticacao.md              # Detalhes de cadastro e recuperação
```

## Pré-requisitos

- SDK **.NET 10**.
- **Node.js 22 LTS** e npm para o frontend Angular 19.
- Servidor **MySQL** compatível com o provedor utilizado e credenciais com permissão para criar/alterar o esquema durante as migrations.
- Ferramenta **dotnet-ef 10**, preferencialmente na versão `10.0.9`, alinhada ao EF Core do projeto.
- Git para clonar e versionar o repositório.
- Chrome ou Chromium para executar os testes do frontend com `ChromeHeadless`.
- Servidor SMTP, caso queira utilizar a recuperação de senha por e-mail.

## Instalação e execução local

Os comandos abaixo partem da raiz do repositório, salvo quando indicado.

### 1. Clonar e restaurar o backend

```bash
git clone https://github.com/fsshall9000-cmd/GFinancas.git
cd GFinancas
dotnet restore GFinancas.slnx
```

### 2. Preparar o MySQL

Crie um banco e utilize um usuário com acesso a ele. Exemplo de criação do banco em um cliente MySQL:

```sql
CREATE DATABASE gfinancas CHARACTER SET utf8mb4;
```

O host, a porta, o usuário e a senha serão informados na configuração da conexão.

### 3. Configurar os segredos locais

O projeto da API já possui `UserSecretsId`. Configure os valores sem gravar credenciais no repositório:

```bash
dotnet user-secrets set 'ConnectionStrings:DefaultConnection' \
  'Server=localhost;Port=3306;Database=gfinancas;User=SEU_USUARIO;Password=SUA_SENHA;' \
  --project src/GFinancas.API

dotnet user-secrets set 'Jwt:Key' \
  'SUBSTITUA_POR_UM_SEGREDO_ALEATORIO_COM_PELO_MENOS_32_BYTES' \
  --project src/GFinancas.API
```

Substitua os exemplos pelos seus valores. A chave JWT deve ser aleatória e ter pelo menos 32 bytes para a assinatura HMAC-SHA256. Os user secrets são carregados no ambiente `Development`.

### 4. Aplicar as migrations

Instale a ferramenta se ainda não estiver disponível:

```bash
dotnet tool install --global dotnet-ef --version 10.0.9
```

Se já houver outra versão instalada, ajuste-a com `dotnet tool update --global dotnet-ef --version 10.0.9`.

Com o MySQL em execução e as configurações preenchidas:

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet ef database update \
  --project src/GFinancas.Infrastructure \
  --startup-project src/GFinancas.API \
  --context AppDbContext
```

A atribuição de variável acima usa a sintaxe de Bash. No PowerShell, configure `$env:ASPNETCORE_ENVIRONMENT = 'Development'` antes de executar `dotnet ef database update` com os mesmos argumentos.

As migrations criam as tabelas e incluem a carga inicial de tipos de conta. A API não aplica migrations automaticamente na inicialização.

### 5. Iniciar a API

```bash
dotnet run --project src/GFinancas.API --launch-profile http
```

| Recurso local | Endereço |
| --- | --- |
| API | `http://localhost:5143/api` |
| Interface Scalar | `http://localhost:5143/scalar/v1` |
| Documento OpenAPI | `http://localhost:5143/openapi/v1.json` |

Scalar e OpenAPI são disponibilizados apenas em `Development`. O perfil alternativo `https` utiliza `https://localhost:7057` e `http://localhost:5143`.

### 6. Instalar e iniciar o frontend

Em outro terminal:

```bash
cd frontend/gfinancas-web
npm ci
npm start
```

Abra **http://localhost:4200**. A URL da API está definida em [`frontend/gfinancas-web/src/enviroments/environment.ts`](frontend/gfinancas-web/src/enviroments/environment.ts):

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5143/api'
};
```

Se alterar a porta ou o endereço da API, ajuste esse arquivo. Se alterar a origem do frontend, configure também o CORS da API.

## Configurações

O arquivo [`appsettings.json`](src/GFinancas.API/appsettings.json) reúne as chaves de configuração. A conexão, a chave JWT e as credenciais SMTP são distribuídas vazias e precisam ser fornecidas para os recursos correspondentes.

| Chave | Variável de ambiente | Finalidade |
| --- | --- | --- |
| `ConnectionStrings:DefaultConnection` | `ConnectionStrings__DefaultConnection` | Conexão com o MySQL. |
| `Jwt:Key` | `Jwt__Key` | Segredo de assinatura e validação dos tokens. |
| `Jwt:Issuer` | `Jwt__Issuer` | Emissor; padrão do projeto: `GFinancas.API`. |
| `Jwt:Audience` | `Jwt__Audience` | Destinatário; padrão: `GFinancas.Angular`. |
| `Cors:AllowedOrigins:0` | `Cors__AllowedOrigins__0` | Origem permitida; sem configuração, aceita `http://localhost:4200`. |
| `RecuperacaoSenha:UrlFrontend` | `RecuperacaoSenha__UrlFrontend` | URL base do frontend, terminando em `/`. |
| `Smtp:Host` | `Smtp__Host` | Servidor de envio de e-mail. |
| `Smtp:Porta` | `Smtp__Porta` | Porta SMTP; padrão: `587`. |
| `Smtp:UsarSsl` | `Smtp__UsarSsl` | Habilita STARTTLS; padrão: `true`. |
| `Smtp:Remetente` | `Smtp__Remetente` | Endereço remetente dos e-mails. |
| `Smtp:Usuario` | `Smtp__Usuario` | Usuário SMTP, quando houver autenticação. |
| `Smtp:Senha` | `Smtp__Senha` | Senha SMTP. |

Para mais origens CORS, utilize os índices `1`, `2` etc. Variáveis de ambiente podem ser usadas em hospedagem; para desenvolvimento, prefira user secrets para os valores sensíveis.

Embora exista `Jwt:ExpirationHours` no arquivo de configuração, a emissão atual fixa a validade em **duas horas** no `TokenService`. Alterar apenas essa configuração não modifica o prazo do token.

## Primeiro acesso

1. Acesse `/login` e selecione **Cadastrar usuário**.
2. Informe nome, e-mail, senha e confirmação. A senha deve ter entre 8 e 128 caracteres.
3. Entre com o e-mail e a senha cadastrados.
4. Cadastre suas contas e categorias.
5. Registre receitas e despesas e consulte o dashboard.
6. Defina orçamentos e cadastre aplicações financeiras conforme a sua necessidade.

O cadastro normaliza novos e-mails para minúsculas e remove espaços das extremidades. A recuperação de senha depende de SMTP configurado; cadastro e login podem ser usados independentemente do envio de e-mail.

Principais rotas da interface: `/dashboard`, `/dashboard/financeiro`, `/dashboard/patrimonio`, `/dashboard/orcamentos`, `/contas`, `/categorias`, `/transacoes`, `/orcamentos` e `/aplicacoes`.

## API REST

Os caminhos abaixo são relativos à base `http://localhost:5143/api`. Para os detalhes dos corpos, parâmetros e respostas, consulte o OpenAPI gerado pela própria API.

### Autenticação e usuários

| Método | Caminho | Uso | Exige JWT |
| --- | --- | --- | --- |
| POST | `/Usuario/Adicionar` | Cadastrar usuário. | Não |
| POST | `/Auth/login` | Obter token e dados do usuário. | Não |
| POST | `/Auth/esqueceu-senha` | Solicitar link de recuperação. | Não |
| POST | `/Auth/redefinir-senha` | Definir senha com token de recuperação. | Não |
| GET | `/Usuario/me` | Consultar o próprio perfil. | Sim |
| GET | `/Usuario/BuscarPorId/{id}` | Consultar usuário por ID. | Sim |
| GET | `/Usuario/Listar` | Listar usuários. | Não, na implementação atual |

### Contas, categorias, transações e orçamentos

Todos os endpoints desta tabela exigem JWT.

| Recurso | Operações |
| --- | --- |
| Conta | `POST /Conta/Adicionar`, `GET /Conta/Listar`, `GET /Conta/BuscarPorId/{id}`, `PUT /Conta/Atualizar/{id}`, `DELETE /Conta/Remover/{id}`, `GET /Conta/{id}/saldo`. |
| Tipo de conta | `GET /TipoConta/Listar`. |
| Categoria | `POST /Categoria/Adicionar`, `GET /Categoria/Listar`, `GET /Categoria/ObterPorId/{id}`, `PUT /Categoria/Atualizar/{id}`, `DELETE /Categoria/Remover/{id}`. |
| Transação | `POST /Transacao/Adicionar`, `GET /Transacao/Listar`, `GET /Transacao/BuscarPorId/{id}`, `PUT /Transacao/Atualizar/{id}`, `DELETE /Transacao/Remover/{id}`, `GET /Transacao/filtro`, `GET /Transacao/paginado`. |
| Orçamento | `POST /Orcamento/Adicionar`, `GET /Orcamento/Listar`, `GET /Orcamento/ObterPorId/{id}`, `PUT /Orcamento/Atualizar/{id}`, `DELETE /Orcamento/Remover/{id}`. |

A consulta paginada recebe `pagina` e `tamanhoPagina`, com padrões `1` e `10`. O filtro de transações aceita critérios como `DataInicio`, `DataFim`, `Tipo`, `ContaId` e `CategoriaId`; o DTO também contém `UsuarioId`. Para os valores de `Tipo`, receita é `1` e despesa é `2`.

### Investimentos e dashboard

Todos os endpoints desta tabela exigem JWT.

| Recurso | Operações |
| --- | --- |
| Aplicação financeira | `POST /AplicacaoFinanceira/Adicionar`, `GET /AplicacaoFinanceira/Listar`, `GET /AplicacaoFinanceira/ObterPorId/{id}`, `PUT /AplicacaoFinanceira/Atualizar/{id}`, `PATCH /AplicacaoFinanceira/AtualizarValor/{id}`, `GET /AplicacaoFinanceira/{id}/HistoricoValores`, `PATCH /AplicacaoFinanceira/Desativar/{id}`. |
| Movimentação de aplicação | `POST /MovimentacaoAplicacao/Registrar`, `GET /MovimentacaoAplicacao/ListarPorAplicacao/{aplicacaoId}`. |
| Dashboard | `GET /Dashboard/resumo`, `GET /Dashboard/Patrimonio`, `GET /Dashboard/despesas-por-categoria`, `GET /Dashboard/historico`, `GET /Dashboard/Investimentos`, `GET /Dashboard/EvolucaoPatrimonial`. |

Os tipos de movimentação de aplicação são aporte (`1`) e resgate (`2`). Os serviços incluem validações de valores, datas, vínculo das entidades e regras de resgate.

### Exemplo de consumo

Cadastrar um usuário:

```bash
curl -X POST http://localhost:5143/api/Usuario/Adicionar \
  -H 'Content-Type: application/json' \
  -d '{"nome":"Pessoa de exemplo","email":"pessoa@example.com","senha":"SenhaExemplo123!"}'
```

Autenticar:

```bash
curl -X POST http://localhost:5143/api/Auth/login \
  -H 'Content-Type: application/json' \
  -d '{"email":"pessoa@example.com","senha":"SenhaExemplo123!"}'
```

A resposta de login contém `token`, `expiracao`, `usuarioId`, `nome` e `email`. Utilize o valor de `token` nas próximas requisições:

```bash
curl http://localhost:5143/api/Usuario/me \
  -H 'Authorization: Bearer SEU_TOKEN_JWT'
```

O tratamento global de exceções converte erros de negócio em respostas no formato Problem Details, com códigos `400` para validação, `401` para falta de autorização, `404` para recurso não encontrado, `409` para conflito e `500` para erros não previstos. A limitação dos endpoints de recuperação pode retornar `429`.

## Autenticação e recuperação de senha

A API valida assinatura, emissor, destinatário e expiração dos JWTs. O frontend guarda o token no `localStorage` e o utiliza por meio do interceptor de autenticação.

Na recuperação, o usuário recebe um link para `/redefinir-senha?token=...`, com validade de **30 minutos**. O token é protegido pelo Data Protection e vinculado ao hash atual da senha. A atualização atômica da senha impede reutilização do link e invalida os links anteriores. A resposta normal à solicitação é a mesma para e-mails cadastrados e inexistentes.

Os endpoints de recuperação permitem cinco requisições por minuto por IP. A troca de senha não revoga JWTs já emitidos; eles permanecem válidos até a expiração.

Para configurar SMTP, verificar o fluxo e conhecer os cuidados com chaves de Data Protection e proxies, consulte [`docs/autenticacao.md`](docs/autenticacao.md).

## Testes e compilação

### Backend

```bash
dotnet build GFinancas.slnx
dotnet test tests/GFinancas.Tests/GFinancas.Tests.csproj
```

Os testes xUnit incluem cálculos financeiros, movimentações de aplicações e validações de resgate.

As verificações de recuperação de senha são um executável separado e não fazem parte da solução `.slnx`:

```bash
dotnet run --project tests/GFinancas.Auth.Checks
```

Esse executável utiliza repositório e remetente falsos, sem enviar e-mails reais. Verifica normalização de e-mail, expiração, token inválido, troca de senha, reutilização e rejeição de atualização concorrente.

### Frontend

Dentro de `frontend/gfinancas-web`:

```bash
npm run build
npm test -- --watch=false --browsers=ChromeHeadless
```

Para desenvolvimento, também estão disponíveis:

```bash
npm run build -- --configuration development
npm run watch
```

Os testes com Karma precisam de Chrome/Chromium instalado. Quando necessário, configure a variável `CHROME_BIN` para o caminho do executável.

## Publicação da aplicação

Para gerar os artefatos da API, a partir da raiz:

```bash
dotnet publish src/GFinancas.API -c Release -o artifacts/api
```

Para gerar o frontend:

```bash
cd frontend/gfinancas-web
npm ci
npm run build
```

O Angular grava os artefatos em `dist/`, conforme `angular.json`. Antes de publicar, ajuste `environment.ts` para o endereço público da API; a configuração atual aponta para localhost.

Na hospedagem, forneça a conexão, a chave JWT e as demais configurações por variáveis de ambiente ou por um gerenciador de segredos. Aplique as migrations ao banco de destino, configure a origem pública em CORS e disponibilize HTTPS. Fora de desenvolvimento, a API habilita redirecionamento para HTTPS.

O servidor de arquivos do frontend deve encaminhar rotas como `/dashboard` e `/redefinir-senha` para `index.html`. Para a recuperação de senha continuar válida entre reinicializações ou múltiplas instâncias, mantenha as chaves do Data Protection persistentes e compartilhadas. Configure também a URL pública de recuperação e o SMTP.

## Solução de problemas

| Sintoma | O que verificar |
| --- | --- |
| API falha ao iniciar ou emitir token | Preencha `Jwt:Key`, confira seu tamanho e a conexão `DefaultConnection`. As chaves vazias do arquivo padrão precisam ser substituídas. |
| Falha de conexão com MySQL | Confirme que o servidor está ativo, a porta está correta e o usuário possui acesso ao banco. |
| Tabelas inexistentes | Execute `dotnet ef database update` com os projetos e contexto indicados na instalação. |
| Comando `dotnet ef` não encontrado | Instale a ferramenta e confira se o diretório de ferramentas globais do .NET está no `PATH`. |
| Frontend não alcança a API | Confira `apiUrl`, a porta da API e as origens permitidas em CORS. |
| Resposta `401` | Faça login novamente e confira o cabeçalho Bearer, a expiração, o emissor e o destinatário do JWT. |
| Recuperação informa serviço não configurado | Preencha `RecuperacaoSenha:UrlFrontend`, `Smtp:Host` e `Smtp:Remetente`, além das credenciais necessárias. |
| Link de recuperação rejeitado | Solicite outro link; o anterior pode ter expirado, sido usado ou invalidado por uma troca de senha/chaveiro. |
| Testes Angular não iniciam o navegador | Instale Chrome/Chromium e configure `CHROME_BIN` quando necessário. |
| Atualizar uma rota do frontend retorna `404` | Configure o servidor estático para encaminhar as rotas da aplicação para `index.html`. |

## Contribuição

1. Crie uma branch para a alteração.
2. Preserve a separação de responsabilidades entre as camadas.
3. Se alterar o modelo persistido, inclua a migration correspondente.
4. Execute as verificações relacionadas ao código modificado.
5. Atualize a documentação e abra um pull request descrevendo a mudança e sua validação.

Não inclua credenciais, tokens, configurações locais sensíveis nem diretórios gerados, como `bin/`, `obj/`, `node_modules/` e `dist/`, nos commits.
