using MySql.Data.MySqlClient;
using System.Text;

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

//�V�K�o�^��ʂ�\�������邽�߂̋@�\
app.MapGet("/new", () =>
{
    return Results.Ok();
});

//�V�K�o�^����
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