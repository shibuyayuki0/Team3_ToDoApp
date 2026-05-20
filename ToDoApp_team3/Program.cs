using ToDoApp_team3.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// ===== カスタム始まり =====

// TaskDataEditorをITaskDataEditorの実装として登録する
// AddScoped：１回分のhttpリクエスト→レスポンスまでの間で存在させる
builder.Services.AddScoped<ITaskDataEditor, TaskDataEditor>();

// ===== カスタム終わり =====

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
