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