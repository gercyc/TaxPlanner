# Features do Frontend — TaxPlanner.Wasm

Documentação por feature (página/rota) do projeto **`TaxPlanner.Wasm`**.

| Feature                | Rota                          | Descrição                                                                  | Documento                                              |
| ---------------------- | ----------------------------- | -------------------------------------------------------------------------- | ------------------------------------------------------ |
| **Home**               | `/`                           | Landing page com visão geral e CTAs.                                       | [`home.md`](./home.md)                                 |
| **Tabelas**            | `/tabelas`                    | Tabelas tributárias de referência.                                          | [`tabelas.md`](./tabelas.md)                           |
| **Análise Tributária** | `/analise-tributaria`         | Cálculos e simulações tributárias.                                          | [`analise-tributaria.md`](./analise-tributaria.md)     |
| **Blog (listagem)**    | `/blog`                       | Lista de posts publicados.                                                  | [`blog-listagem.md`](./blog-listagem.md)               |
| **Blog (post)**        | `/post/{slug}`                | Página de post individual (renderiza Markdown/HTML).                        | [`blog-post.md`](./blog-post.md)                       |
| **404 / NotFound**     | `*`                           | Página de erro para rotas não encontradas.                                  | [`nao-encontrado.md`](./nao-encontrado.md)             |

## Convenção

Cada arquivo de feature segue o esqueleto abaixo:

1. **Propósito** — o que a feature entrega ao usuário.
2. **Rota** — caminho e modo de roteamento.
3. **Componentes** — arquivos `.razor` envolvidos.
4. **Integrações** — endpoints consumidos (`/api/*`).
5. **Estado** — variáveis de componente, parâmetros, lifecycle.
6. **Acessibilidade** — semântica, ARIA, navegação por teclado.
7. **Referências** — links para código e design system.
