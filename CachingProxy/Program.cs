using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.IO;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var cacheDir = Path.Combine(Directory.GetCurrentDirectory(), ".cache");

if (args.Contains("--clear-cache"))
{
    if (Directory.Exists(cacheDir))
    {
        Directory.Delete(cacheDir, true);
        Console.WriteLine("Cache limpo com sucesso!");
    }
    else
    {
        Console.WriteLine("Nenhum cache encontrado para limpar.");
    }
    return;
}

var porta = args.SkipWhile(x => x != "--port").Skip(1).FirstOrDefault() ?? "3000";
var origin = args.SkipWhile(x => x != "--origin").Skip(1).FirstOrDefault() ?? "http://localhost";

if (!Directory.Exists(cacheDir))
{
    Directory.CreateDirectory(cacheDir);
}

app.Map("/{**path}", async (string path, HttpContext context) =>
{
    var safeFileName = path.Replace('/', '_') + ".txt";
    var cacheFilePath = Path.Combine(cacheDir, safeFileName);

    // Se o arquivo de cache já existe, retorna HIT e lê a string do arquivo
    if (File.Exists(cacheFilePath))
    {
        context.Response.Headers["X-Cache"] = "HIT";
        var cachedContent = await File.ReadAllTextAsync(cacheFilePath);
        await context.Response.WriteAsync(cachedContent);
        return;
    }

    // Se não existe (MISS), busca na origem
    var targetUrl = $"{origin}/{path}";
    using var httpClient = new HttpClient();

    var responseFromOrigin = await httpClient.GetAsync(targetUrl);
    var content = await responseFromOrigin.Content.ReadAsStringAsync();

    // Salva a resposta em um arquivo no disco para os próximos acessos
    await File.WriteAllTextAsync(cacheFilePath, content);

    context.Response.Headers["X-Cache"] = "MISS";
    await context.Response.WriteAsync(content);
});

app.Run($"http://localhost:{porta}");