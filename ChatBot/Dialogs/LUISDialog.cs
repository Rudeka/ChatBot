using System;
using System.Linq;
using System.Threading.Tasks;
using ChatBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace ChatBot.Dialogs
{
    [LuisModel("fd3e39d5-5f90-42b1-b78a-657b116ebc4e", "b16f2f5169f5469dbe7c4474becd4c56")]
    [Serializable]
    public class LUISDialog : LuisDialog<ItemReservation>
    {
        private readonly BuildFormDelegate<ItemReservation> ReserveItem;

        public LUISDialog(BuildFormDelegate<ItemReservation> reserveItem)
        {
            ReserveItem = reserveItem;
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I'm sorry I don't know what you mean.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            context.Call(new GreetingDialog(), Callback);
        }

        [LuisIntent("ItemReservation")]
        public async Task ItemReservation(IDialogContext context, LuisResult result)
        {
            var enrollmentForm = new FormDialog<ItemReservation>(new ItemReservation(), ReserveItem, FormOptions.PromptInStart);
            context.Call(enrollmentForm, Callback);
        }

        [LuisIntent("QueryServices")]
        public async Task QueryServices(IDialogContext context, LuisResult result)
        {
            foreach (var entity in result.Entities.Where(e => e.Type == "Service"))
            {
                var value = entity.Entity.ToLower();
                if (value == "delivery" || value == "furniture assembly"
                    || value == "materials selection support" || value == "design")
                {
                    await context.PostAsync("Yes we do that!");
                    context.Wait(MessageReceived);
                    return;
                }
                //await context.PostAsync("I'm sorry we don't do that.");
                //context.Wait(MessageReceived);
                //return;
            }
            await context.PostAsync("I'm sorry we don't do that.");
            context.Wait(MessageReceived);
        }

        private async Task Callback(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceived);
        }
    }
}