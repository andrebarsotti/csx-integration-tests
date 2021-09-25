# Exemplo de Teste de Integração com C# Script

## 1. Sobre esse script

Esse é um script de exemplo mostrando como executar testes restritos de integração (_narrow integration tests_) para um repostiório de um Todo List.

Ele funciona como uma POC de integração entre um Assembly e o script, também como um estudo de caso para realização de testes de unidade em C# scripts.

## 2. Requisitos

Para a execução desse projeto é preciso ter instalado:

- Banco de dados SQL Server (MSLocalDB ou qualquer outra instância local);
- .NET 5.0 ou superior.
- Dot.Net Script instalado nos tools.

## 3. Configuração

1. Na raiz do diretório src/TodoListRepositories compilar o projeto do repositório com o comando:

    ~~~ Shell
    dotnet build
    ~~~

2. Alterar o arquivo _appsettings.json_ da raiz do projeto para incluir a string de connecção do banco sql, abaixo alguns exemplos:

    Instância local:

    ~~~ JSON
    {
        "ConnectionStrings": {
            "db": "Server=localhost;Database=todo-list;User Id=sa;Password=P@ssw0rd"
        }
    }
    ~~~

    LocalDB:

    ~~~ JSON
    {
        "ConnectionStrings": {
            "db": "Server=(LocalDb)\\MSSQLLocalDB;Database=todo-list;Integrated Security=true;"
        }
    }
    ~~~

3. Caso seja necessário instalar o dotnet script utilize o comando:

    ~~~ Shell
    dotnet tool install -g dotnet-script
    ~~~


## 4. Execução

Para executar o script basta rodar o comando abaixo na raiz do projeto:

~~~ Shell
dotnet script --isolated-load-context main.csx
~~~

Um resultado semelhante ao abaixo deve ser apresentado:

~~~ Shell
TodoListRepositoryTests.AddTask_Success         521ms
TodoListRepositoryTests.UpdateTask_Success      129ms
TodoListRepositoryTests.DeleteTask_Success      14ms
Total tests: 3. Passed: 3. Failed: 0.
Test Run Successful.
Test execution time 1.9674748 seconds
~~~


## Referências

FOWLER, Martin. IntegrationTest. martimFowler.com, 2018. Disponível em <[https://martinfowler.com/bliki/IntegrationTest.html](https://martinfowler.com/bliki/IntegrationTest.html)>. Acesso em: 25-09-2021

RICHTER, Bernhard. ScriptUnit. Github, 2018. Disponível em <[https://github.com/seesharper/ScriptUnit]>(https://github.com/seesharper/ScriptUnit)>. Acesso em: 25-09-2021


WOJCIESZYN, Filip. Dotnet script. Github, 2021. Disponível em <[https://github.com/filipw/dotnet-script](https://github.com/filipw/dotnet-script)>. Acesso em: 11-09-2021
