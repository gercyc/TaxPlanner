# Módulos do Backend — TaxPlanner.Server

Documentação por módulo (feature de backend) do projeto **`TaxPlanner.Server`**.

| Módulo                       | Descrição                                                                     | Documento                                                  |
| ---------------------------- | ----------------------------------------------------------------------------- | ---------------------------------------------------------- |
| **Blog (público)**           | Endpoints públicos de leitura e feed RSS.                                     | [`blog-publico.md`](./blog-publico.md)                     |
| **Blog (admin)**             | CRUD administrativo de posts, publicação e gestão de imagens.                 | [`blog-admin.md`](./blog-admin.md)                         |
| **Identidade (Identity)**    | Autenticação por cookie, passkeys, registro e gestão de usuários.             | [`identidade.md`](./identidade.md)                         |

## Convenção

Cada arquivo de módulo segue o esqueleto abaixo:

1. **Propósito** — o que o módulo faz e por que existe.
2. **Pontos de entrada** — controllers, componentes, rotas expostas.
3. **Serviços e dependências** — DbContext, serviços injetados, libs externas.
4. **Modelo de dados** — entidades principais, migrations associadas.
5. **Contratos** — DTOs/records públicos, métodos expostos, autenticação.
6. **Falhas conhecidas** — limitações e dívidas técnicas.
7. **Referências** — links para código e demais documentos.
