# ACPF - Aplicativo de Controle Financeiro Pessoal

Um aplicativo desktop desenvolvido em C# com WPF para controle financeiro pessoal, seguindo o padrão MVVM e utilizando Entity Framework Core com SQLite.

## 🚀 Características

- **Interface Moderna**: Design limpo e responsivo
- **Arquitetura MVVM**: Separação clara entre Model, View e ViewModel
- **Entity Framework Core**: ORM para persistência de dados com SQLite
- **LINQ**: Consultas eficientes e relatórios avançados
- **Injeção de Dependência**: Código limpo e testável
- **Validação de Dados**: Validações robustas em todas as camadas

## 📋 Funcionalidades

### 💰 Gerenciamento de Transações
- Adicionar, editar e excluir transações
- Categorização automática por tipo (Receita/Despesa)
- Filtros por data e categoria
- Observações detalhadas

### 📁 Gerenciamento de Categorias
- Categorias personalizadas para receitas e despesas
- Cores para identificação visual
- Proteção contra exclusão de categorias em uso

### 📊 Relatórios e Análises
- Resumo financeiro com totais
- Relatórios por categoria
- Filtros por período
- Exportação de dados

## 🛠️ Tecnologias Utilizadas

- **.NET 8.0**: Framework principal
- **WPF (Windows Presentation Foundation)**: Interface de usuário
- **Entity Framework Core**: ORM para acesso a dados
- **SQLite**: Banco de dados local
- **LINQ**: Consultas e manipulação de dados
- **MVVM Pattern**: Arquitetura da aplicação
- **Dependency Injection**: Inversão de controle

## 📁 Estrutura do Projeto

```
ACPF/
├── ACPF.Core/                 # Camada de domínio
│   ├── Enums/
│   │   └── TipoTransacao.cs
│   └── Models/
│       ├── Categoria.cs
│       └── Transacao.cs
├── ACPF.DataAccess/           # Camada de acesso a dados
│   └── AppDbContext.cs
├── ACPF.UI/                   # Camada de apresentação
│   ├── ViewModels/
│   │   ├── BaseViewModel.cs
│   │   ├── RelayCommand.cs
│   │   ├── MainViewModel.cs
│   │   ├── TransacaoViewModel.cs
│   │   ├── CategoriaViewModel.cs
│   │   └── RelatorioViewModel.cs
│   ├── Views/
│   │   └── MainWindow.xaml
│   ├── Converters/
│   │   └── SaldoColorConverter.cs
│   ├── App.xaml
│   └── App.xaml.cs
└── ACPF.sln                   # Solução do Visual Studio
```

## 🚀 Como Executar

### Pré-requisitos
- Visual Studio 2022 ou superior
- .NET 8.0 SDK
- SQLite (incluído no projeto)

### Passos para Execução

1. **Clone o repositório**

2. **Abra a solução no Visual Studio**
   - Abra o arquivo `ACPF.sln` no Visual Studio
   - Restaure os pacotes NuGet automaticamente

3. **Configure o banco de dados**
   - O banco SQLite será criado automaticamente na primeira execução
   - As categorias padrão serão inseridas automaticamente

4. **Execute a aplicação**
   - Defina `ACPF.UI` como projeto de inicialização
   - Pressione F5 ou clique em "Iniciar"

## 📖 Como Usar

### Primeira Execução
1. A aplicação será iniciada com categorias padrão já configuradas
2. O banco de dados `financas.db` será criado automaticamente

### Adicionando Transações
1. Navegue para a aba "Transações"
2. Preencha os campos:
   - **Descrição**: Nome da transação
   - **Valor**: Valor em reais
   - **Data**: Data da transação
   - **Tipo**: Receita ou Despesa
   - **Categoria**: Selecione uma categoria
   - **Observações**: Detalhes adicionais (opcional)
3. Clique em "Adicionar"

### Gerenciando Categorias
1. Navegue para a aba "Categorias"
2. Para adicionar uma nova categoria:
   - Preencha o nome e descrição
   - Selecione o tipo (Receita/Despesa)
   - Escolha uma cor (formato #RRGGBB)
   - Clique em "Adicionar"

### Gerando Relatórios
1. Navegue para a aba "Relatórios"
2. Configure os filtros:
   - **Período**: Data início e fim
   - **Categoria**: Filtro opcional por categoria
3. Clique em "Gerar Relatório"
4. Visualize os resultados e exporte se necessário

## 🔧 Configurações Avançadas

### Personalizando Categorias
- As categorias padrão podem ser editadas ou removidas
- Novas categorias podem ser criadas conforme necessário
- Categorias em uso não podem ser excluídas (apenas desativadas)

### Backup dos Dados
- O arquivo `financas.db` contém todos os dados
- Faça backup regular deste arquivo
- Para restaurar, substitua o arquivo existente

## 🐛 Problemas

### Erro de Conexão com Banco
- Verifique se o arquivo `financas.db` não está sendo usado por outro processo
- Delete o arquivo para recriar o banco (dados serão perdidos)

### Interface Não Carrega
- Verifique se todos os pacotes NuGet foram restaurados
- Recompile a solução completa

### Dados Não Aparecem
- Verifique se o banco foi criado corretamente
- Use o comando "Gerar Relatório" para forçar o carregamento

### Editar
- Editar não funcionando corretamente

## 📝 Licença

Este projeto é fornecido como exemplo educacional. Sinta-se livre para usar, modificar e distribuir conforme suas necessidades.

## 🤝 Contribuições

Contribuições são bem-vindas! Para contribuir:

1. Faça um fork do projeto
2. Crie uma branch para sua feature
3. Commit suas mudanças
4. Push para a branch
5. Abra um Pull Request

## 📞 Suporte

Para dúvidas ou problemas:
- Abra uma issue no repositório
- Consulte a documentação do Entity Framework Core
- Verifique a documentação do WPF

---

**Desenvolvido com ❤️ usando C# e WPF** 