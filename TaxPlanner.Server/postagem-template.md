# Como Simular seu Imposto de Renda com o TaxPlanner

## O que é o TaxPlanner?

O **TaxPlanner** é um simulador gratuito de Imposto de Renda para pessoas físicas, baseado nas tabelas oficiais da Receita Federal. Ele permite que você:

- Informe seus **salários ao longo do ano**, incluindo promoções e reajustes
- Descubra se terá **restituição** ou **imposto a pagar**
- Veja o detalhamento **mês a mês** dos descontos de INSS e IRRF
- Simule o impacto do **13º salário** no cálculo final

Tudo isso de forma **100% privada** — os cálculos acontecem no seu navegador, sem cadastro e sem armazenamento de dados.

---

## Passo a Passo

A simulação segue **4 etapas simples**:

### Passo 1 — Escolha o ano-calendário

Na tela de [Análise Tributária](/analise-tributaria), o primeiro campo é o **ano-calendário** — o ano em que os rendimentos foram (ou serão) recebidos.

| Ano | Tabela utilizada |
|-----|-----------------|
| 2020 a 2024 | Tabela oficial da Receita Federal para o ano |
| 2025 em diante | Última tabela disponível como estimativa |

> ⚠️ Para anos futuros, o TaxPlanner exibe um aviso informando que os valores são estimativas. As tabelas oficiais são carregadas assim que publicadas pela Receita Federal.

Ative também a opção **"Incluir 13º salário no cálculo"** se você recebe esse valor. O 13º é tributado de forma exclusiva na fonte e aparece separado no resultado.

---

### Passo 2 — Informe seus períodos salariais

Esta é a principal funcionalidade do TaxPlanner: você pode adicionar **múltiplos períodos** com salários diferentes.

Isso é útil quando:

- Você recebeu um **aumento** no meio do ano
- Foi **promovido** e passou a ganhar um novo salário
- Teve **reajuste sindical** ou dissídio em um mês específico
- Mudou de **emprego** e teve salários diferentes em cada empresa

**Exemplo prático:**

| Período | Meses | Salário bruto |
|---------|-------|---------------|
| Jan a Jun | 6 meses | R$ 6.000,00 |
| Jul a Dez | 6 meses | R$ 8.500,00 |

Basta clicar em **"Adicionar Período"**, preencher as datas de início e fim, informar o salário bruto de cada fase e repetir quantas vezes precisar.

O sistema valida automaticamente se há sobreposição de datas ou salário zerado e avisa antes do cálculo.

---

### Passo 3 — Execute o cálculo

Com os períodos preenchidos, clique em **"Calcular"**. O TaxPlanner processa:

1. **Mês a mês**: para cada mês do ano, calcula o INSS com a tabela vigente, determina a base de cálculo e aplica a alíquota de IRRF correspondente
2. **13º salário** (se ativado): aplica a tributação exclusiva na fonte
3. **Consolidação anual**: soma todos os rendimentos, descontos e retenções, e aplica a tabela progressiva anual para determinar o imposto devido

O resultado aparece em **tempo real**, sem recarregar a página.

---

### Passo 4 — Entenda o resultado

O resultado é apresentado em três níveis de detalhe:

#### Resumo anual

| Campo | O que significa |
|-------|----------------|
| **Rendimento bruto anual** | Soma de todos os salários informados |
| **INSS anual** | Total descontado de previdência |
| **Base de cálculo anual** | Rendimento bruto menos INSS |
| **IR devido anual** | Imposto calculado pela tabela progressiva |
| **IRRF retido anual** | Imposto já retido na fonte mês a mês |

#### Detalhamento mensal

Uma tabela com **cada mês do ano** mostrando salário bruto, INSS, base de cálculo e IRRF retido. Ideal para conferir os descontos mês a mês e identificar distorções.

#### Resultado final

```
Resultado positivo → IMPOSTO A PAGAR 💰
Resultado negativo → RESTITUIÇÃO ✅
```

> 💡 **Dica**: se o resultado for positivo (a pagar), você pode avaliar se vale a pena fazer deduções via plano de previdência (PGBL) ou contribuir com despesas dedutíveis antes do fim do ano.

---

## Casos de Uso Comuns

### Simular antes da declaração anual

Selecione o ano anterior, informe seus salários exatamente como foram, e descubra com antecedência se você terá imposto a pagar ou restituição.

### Planejar uma promoção

Adicione dois períodos — antes e depois da promoção — e veja o impacto real do novo salário na sua carga tributária. O aumento pode fazer você mudar de faixa da tabela progressiva.

### Comparar anos diferentes

Altere o ano-calendário e mantenha os mesmos períodos. As diferenças na tabela do IR e do INSS de um ano para outro podem surpreender.

### Entender o impacto do 13º

Execute a simulação duas vezes: uma com o 13º ativado e outra sem. Compare os resultados para entender exatamente quanto do seu 13º vai para o imposto.

---

## Limitações

O TaxPlanner é uma **ferramenta de estimativa**. Ele não considera:

- Deduções com **saúde**, **educação** ou **dependentes**
- **Pensão alimentícia**
- Rendimentos de outras fontes (aluguéis, investimentos, pró-labore)
- **Despesas do livro-caixa** para profissionais autônomos

> ⚠️ Para uma análise tributária completa, **consulte um contador**. O TaxPlanner é um ponto de partida, não substitui um profissional.

---

## Privacidade

Todos os cálculos do TaxPlanner são processados **exclusivamente no seu navegador**, usando WebAssembly (.NET executado no cliente). Isso significa que:

- ❌ Nenhum dado salarial é enviado a servidores
- ❌ Nenhum cadastro é necessário
- ❌ Nenhum cookie de rastreamento
- ✅ Você pode fechar o navegador e seus dados desaparecem

---

## Experimente agora

Acesse a [Análise Tributária](/analise-tributaria) e faça sua primeira simulação em menos de 2 minutos.

*Publicado em maio de 2025*
