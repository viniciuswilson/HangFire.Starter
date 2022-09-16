using Hangfire;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HangFire.Starter
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddHangfire(op =>
            {
                op.UseMemoryStorage();
            });
            services.AddHangfireServer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            //**************************************************************************************************
            //  Referência do cron https://en.wikipedia.org/wiki/Cron
            //**************************************************************************************************
            //  field #   meaning        allowed values
            //# -------   ------------   --------------
            //#    1      minute         0-59
            //#    2      hour           0-23
            //#    3      day of month   1-31
            //#    4      month          1-12 (or names, see below)
            //#    5      day of week    0-7 (0 or 7 is Sun, or use names)
            //**************************************************************************************************
            //
            // HORÁRIO UTC => +3 HORAS QUE BRASÍLIA, EXEMPLO:
            //          PARA RODAR ÀS 6 HRS DE BRASÍLIA DEVE CONFIGURAR O CRON COM 9HS
            //
            //**************************************************************************************************

            //BackgroundJob.Enqueue(() => AbrirAgendasCriadas());

            //Uma única vez. Se for rodar novamente, alterar os parâmetros do método
            //BackgroundJob.Schedule(() => SelecaoProgramaSocial(1, new DateTime(2021, 9, 6)), new DateTime(2021, 9, 6, 00, 00, 00));

            
            BackgroundJob.Enqueue(() => MeuPrimeiroJobFireAndForget());

            RecurringJob.AddOrUpdate(() => RecurringJobs1(), Cron.Minutely);

            //Diariamente às 09:00h
            RecurringJob.AddOrUpdate(() => RecurringJobs1(), "00 12 * * *");

            BackgroundJob.Schedule(() => RecurringJobs1(), TimeSpan.FromDays(2));

            String JobId = BackgroundJob.Enqueue(() => TarefaPai());
            BackgroundJob.ContinueJobWith(JobId, () => TarefaFilha());

            app.UseHangfireDashboard();

        }
        public async Task MeuPrimeiroJobFireAndForget()
        {
            await Task.Run(() =>
            {
               // throw new Exception("Algo deu Errado");
                Console.WriteLine("Bem Vindo ao HangFire !");
            });
        }

        public async Task RecurringJobs1()
        {
            await Task.Run(() =>
            {
                 Console.WriteLine("Tarefa recorente!");
            });
        }

        public async Task TarefaPai()
        {
            await Task.Run(() =>
            {
                Console.WriteLine("Tarefa TarefaPai!");
            });
        }

        public async Task TarefaFilha()
        {
            await Task.Run(() =>
            {
                Console.WriteLine("Tarefa TarefaFilha!");
            });
        }
    }
}
