using MySql.Data.MySqlClient;

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

//MySQL接続
MySqlConnection connection = new MySqlConnection("server=localhost;user=root;password=Malaysia4649;database=message_information;");


app.MapGet("/index", (int? page) =>
{
    if (page == null)
    {
        connection.Open();
        MySqlCommand command = new MySqlCommand("select id, message from messages limit 0,5;", connection);
        MySqlDataReader reader = command.ExecuteReader();

        var resultList = new List<Message>();
        while (reader.Read())
        {
            resultList.Add(new Message { id = reader.GetInt32("id"), message = reader.GetString("message") });
        }
        reader.Close();


        MySqlCommand messageListCount = new MySqlCommand("select count(id)  from messages ;", connection);
        int[] pages = new int[1];
        pages[0] = 1;

        connection.Close();
        return Results.Ok(new { resultList, pages });
    }
    else
    {
        connection.Open();
        MySqlCommand command = new MySqlCommand("select id, message from messages limit @start,5;", connection);
        command.Parameters.Add(new MySqlParameter("@start", page * 5 - 5));
        MySqlDataReader reader = command.ExecuteReader();

        var resultList = new List<Message>();
        while (reader.Read())
        {
            resultList.Add(new Message { id = reader.GetInt32("id"), message = reader.GetString("message") });
        }
        reader.Close();


        MySqlCommand messageListCount = new MySqlCommand("select count(id)  from messages ;", connection);
        double doubleOfTotalPageCount = Convert.ToDouble(messageListCount.ExecuteScalar()) / 5;
        int TotalPageCount = (int)Math.Ceiling(doubleOfTotalPageCount);
        int[] pages = new int[TotalPageCount];
        int j = 1;
        for (int i = 0; i < pages.Length; i++)
        {

            pages[i] = j;

            j++;

        }
        connection.Close();
        return Results.Ok(new { resultList, pages });
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
    connection.Open();
    MySqlCommand command = new MySqlCommand("insert into messages (message)  values (@message);", connection);
    command.Parameters.AddWithValue("@message", mes.message);
    command.ExecuteNonQuery();

    //以下一覧表示と同じ処理のため、省略できないかな
    command = new MySqlCommand("select id, message from messages;", connection);
    MySqlDataReader reader = command.ExecuteReader();

    var resultList = new List<Message>();
    while (reader.Read())
    {
        resultList.Add(new Message { id = reader.GetInt32("id"), message = reader.GetString("message") });
    }
    reader.Close();
    connection.Close();
    return Results.Ok(resultList);


});

//ID指定してメッセージを獲得する
app.MapGet("/show", (int? id) =>
{
    connection.Open();
    MySqlCommand command = new MySqlCommand("select id, message from messages where id= @id ;", connection);
    command.Parameters.Add(new MySqlParameter("@id", id));
    MySqlDataReader reader = command.ExecuteReader();
    reader.Read();
    Message mes = new Message { id = reader.GetInt32("id"), message = reader.GetString("message") };
    reader.Close();
    connection.Close();
    return Results.Ok(mes);
});

//編集画面を表示させる
app.MapGet("/edit", () =>
{
    return Results.Ok();
});

//更新処理
app.MapPost("/update", (Message mes) =>
{
    connection.Open();
    MySqlCommand command = new MySqlCommand("update messages set message=@message where id = @id;", connection);
    command.Parameters.Add(new MySqlParameter("@id", mes.id));
    command.Parameters.Add(new MySqlParameter("@message", mes.message));
    command.ExecuteNonQuery();

    //以下一覧表示と同じ処理のため、省略できないかな
    command = new MySqlCommand("select id, message from messages;", connection);
    MySqlDataReader reader = command.ExecuteReader();

    var resultList = new List<Message>();
    while (reader.Read())
    {
        resultList.Add(new Message { id = reader.GetInt32("id"), message = reader.GetString("message") });
    }
    reader.Close();
    connection.Close();
    return Results.Ok(resultList);

});

app.MapPost("/delete", (Message mes) =>
{
    connection.Open();
    MySqlCommand command = new MySqlCommand("delete from  messages where id = @id;", connection);
    command.Parameters.Add(new MySqlParameter("@id", mes.id));
    command.ExecuteNonQuery();

    //以下一覧表示と同じ処理のため、省略できないかな
    command = new MySqlCommand("select id, message from messages;", connection);
    MySqlDataReader reader = command.ExecuteReader();

    var resultList = new List<Message>();
    while (reader.Read())
    {
        resultList.Add(new Message { id = reader.GetInt32("id"), message = reader.GetString("message") });
    }
    reader.Close();
    connection.Close();
    return Results.Ok(resultList);

});


app.Run();

class Message
{
    public int? id { get; set; }
    public string? message { get; set; }
}