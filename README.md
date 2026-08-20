# 📡 Coletor de Logs de Roteadores

Aplicação em C# desenvolvida para automatizar a coleta de logs de roteadores em redes WAN / MAN

---

## 📌 Sobre o Projeto

Durante o meu estágio, a coleta de logs dos roteadores da rede era um processo totalmente manual. Cada consulta exigia acesso individual via SSH/CLI, a execução manual dos comandos e a cópia dos dados para arquivos locais. Esse fluxo era repetitivo, consumia um tempo considerável da equipe e abria margem para erros operacionais. Para resolver esse problema, desenvolvi este Coletor de Logs, uma ferramenta automatizada que se conecta aos dispositivos, executa os comandos necessários e exporta os registros de forma organizada.

---

## ⚡ Funcionalidades

- **Suporte Multivendor:** Compatível com roteadores Cisco, Huawei. (Alcatel (Nokia) futuramente).
- **Conexão Automatizada:** Acesso aos ativos de rede via protocolo SSH.
- **Exportação Estruturada:** Salvamento automático dos registros em arquivos `.txt`.
- **Padronização:** Organização dos arquivos por formato predefinido para facilitar auditorias e análises.
- **Redução de Falhas:** Eliminação de erros manuais no processo de captura de logs.

---

## 🛠️ Tecnologias Utilizadas

- **Linguagem:** C# (.NET)
- **Protocolo de Conexão:** SSH (SSH.NET)
- **Arquitetura:** Orientação a Objetos com separação por Entidades e DTOs

---

## 🚀 Como Executar o Projeto

### Pré-requisitos
- .NET SDK (versão 6.0 ou superior)
- Conectividade de rede/VPN com alcance aos IPs dos roteadores e possibilidade de conexão remota

### Passos para execução
1. Clone o repositório:
   ```bash
   git clone [https://github.com/seu-usuario/nome-do-repositorio.git](https://github.com/seu-usuario/nome-do-repositorio.git)

2. Crie um arquivo chamado `LCollector.txt` na raiz do projeto e insira os endereços IP dos dispositivos desejados, inserindo um por linha

3. Executar no terminal com `dotnet run`
