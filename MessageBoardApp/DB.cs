using MySql.Data.MySqlClient;
using System.Reflection.PortableExecutable;
using System.Text;

namespace MySQLConnectionTest
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

//全件取得
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
    }

});

//新規登録画面を表示させるための機能
app.MapGet("/new", () =>
{
return Results.Ok();
});
app.Run();

class Response
{
    public int? Id { get; set; }
    public string? Message { get; set; }
}