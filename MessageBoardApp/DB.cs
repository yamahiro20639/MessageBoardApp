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
        var resultList = new List<Message>();


        while (reader.Read())
        {
            resultList.Add(new Message { id = reader.GetInt32("id"), message = reader.GetString("message") });
        }
        return Results.Ok(resultList);
    }

});

//新規登録画面を表示させるための機能
app.MapGet("/new", () =>
{
    return Results.Ok();
});

//新規登録処理
app.MapPost("/create", (Message mes) =>
{
    using (var con = new MySqlConnection("server=localhost;user=root;password=Malaysia4649;database=message_information;"))
    {
        con.Open();
        var command = new MySqlCommand("insert into messages (message)  values (@message);", con);
        command.Parameters.AddWithValue("@message", mes.message);
        command.ExecuteNonQuery();
        command = new MySqlCommand("select id, message from messages;", con);
        var reader = command.ExecuteReader();
        var resultList = new List<Message>();


        while (reader.Read())
        {
            resultList.Add(new Message { id = reader.GetInt32("id"), message = reader.GetString("message") });
        }
        return Results.Ok(resultList);
    }

});


app.Run();

class Message
{
    public int? id { get; set; }
    public string? message { get; set; }
}