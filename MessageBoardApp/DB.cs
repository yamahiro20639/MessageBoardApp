using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()   // ���ׂẴI���W������̃A�N�Z�X������
                   .AllowAnyMethod()
                   .AllowAnyHeader()
            .WithExposedHeaders("Content-Disposition") // �ǉ�: �K�v�ɉ����Č��J�w�b�_�[��ݒ�
                   .SetIsOriginAllowed((host) => true); // �ǉ�: �I���W���̋��𓮓I�ɐݒ�
        });
});

var app = builder.Build();
// CORS�|���V�[��ݒ�
app.UseCors("AllowAll");

//MySQL�ڑ�
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

//�V�K�o�^��ʂ�\�������邽�߂̋@�\
app.MapGet("/new", () =>
{
    return Results.Ok();
});

//�V�K�o�^����
app.MapPost("/create", (Message mes) =>
{
    connection.Open();
    MySqlCommand command = new MySqlCommand("insert into messages (message)  values (@message);", connection);
    command.Parameters.AddWithValue("@message", mes.message);
    command.ExecuteNonQuery();

    //�ȉ��ꗗ�\���Ɠ��������̂��߁A�ȗ��ł��Ȃ�����
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

//ID�w�肵�ă��b�Z�[�W���l������
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

//�ҏW��ʂ�\��������
app.MapGet("/edit", () =>
{
    return Results.Ok();
});

//�X�V����
app.MapPost("/update", (Message mes) =>
{
    connection.Open();
    MySqlCommand command = new MySqlCommand("update messages set message=@message where id = @id;", connection);
    command.Parameters.Add(new MySqlParameter("@id", mes.id));
    command.Parameters.Add(new MySqlParameter("@message", mes.message));
    command.ExecuteNonQuery();

    //�ȉ��ꗗ�\���Ɠ��������̂��߁A�ȗ��ł��Ȃ�����
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

    //�ȉ��ꗗ�\���Ɠ��������̂��߁A�ȗ��ł��Ȃ�����
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