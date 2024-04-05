using MySql.Data.MySqlClient;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()   // すべてのオリジンからのアクセスを許可
                   .AllowAnyMethod()
                   .AllowAnyHeader()
            .WithExposedHeaders("Content-Disposition") // 追加: 必要に応じて公開ヘッダーを設定
                   .SetIsOriginAllowed((host) => true); // 追加: オリジンの許可を動的に設定
        });
});

var app = builder.Build();
// CORSポリシーを設定
app.UseCors("AllowAll");

app.MapGet("/index", () =>
{
    using (var con = new MySqlConnection("server=localhost;user=root;password=Malaysia4649;database=message_information;"))
    {
        con.Open();
        var command = new MySqlCommand("select id, message from messages;", con);
        var reader = command.ExecuteReader();
        var resultList = new List<Response>();


        while (reader.Read())
        {
            resultList.Add(new Response { Id = reader.GetInt32("id"), Message = reader.GetString("message") });
        }
        return Results.Ok(resultList);
    }

});

app.Run();

class Response
{
    public int? Id { get; set; }
    public string? Message { get; set; }
}