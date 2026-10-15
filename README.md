# Caching Proxy

Um servidor proxy reverso  construído em **C# / .NET (Minimal APIs)** que intercepta requisições HTTP, repassa para um servidor de origem (*origin*) e armazena as respostas em cache para acelerar acessos futuros e economizar banda.

## ⚙️ Como Executar

### 1. Clonar o Repositório e Acessar a Pasta
```bash
git clone https://github.com/seu-usuario/CachingProxy.git
cd CachingProxy
```

### 2. Iniciar o Servidor Proxy
Para rodar o proxy apontando para uma API de exemplo (como a DummyJSON) em uma porta específica (ex: `8080`):

```bash
dotnet run -- --port 8080 --origin https://dummyjson.com
```

### 3. Limpar o Cache
Caso precise limpar os registros salvos:

```bash
dotnet run -- --clear-cache
```

---

### Como Testar

Com o servidor rodando em um terminal, faça uma requisição de teste usando o curl em outro terminal:

```bash
curl -i http://localhost:8080/products/1
```

* **Primeira requisição:** Irá buscar na internet e retornar `X-Cache: MISS`.
* **Segunda requisição (repetida):** Irá retornar instantaneamente da memória com `X-Cache: HIT`.
