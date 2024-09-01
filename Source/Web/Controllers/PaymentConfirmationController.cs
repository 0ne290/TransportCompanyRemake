using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Dtos;
using Order = Domain.Entities.Order;

namespace Web.Controllers;

// TODO: Этот контроллер лежит вне моей "архитектуры" и не соответствует принципам Clean Architecture и разделения ответственности
[Route("payment-confirmation")]
public class PaymentConfirmationController(IEntityStorageService<Order> orderStorageService, IOrderPaymentService orderPaymentService) : Controller
{
    // TODO: Перенести модель уведомления ЮКассы и всю логику в Application Layer. Это действие контроллера ASP.NET должно просто вызывать метод из Application Layer и возвращать тот или иной HTTP-код в зависимости от результата этого метода
    [HttpPost]
    [Route("yookassa")]
    public async Task<IActionResult> YooKassa([FromBody] YooKassaNotification yooKassaNotification)
    {
        // В документации ЮКассы написано, что единственный способ удостовериться в том, что уведомление прислала
        // именно ЮКасса, это проверить, совпадает ли IP адрес источника уведомления с каким-нибудь из описанных
        // в документации. Но мне прислали уведомление с какого-то вообще левого IP-адреса, НЕ описанного в
        // документации. Вывод: удостовериться в том, что уведомление прислала именно ЮКасса, похоже, невозможно
        // var clientIp = HttpContext.Connection.RemoteIpAddress!.ToString();
        // if (clientIp != "185.71.76.0/27" && clientIp != "185.71.77.0/27" && clientIp != "77.75.153.0/25" &&
        //     clientIp != "77.75.156.11" && clientIp != "77.75.156.35" && clientIp != "77.75.154.128/25" &&
        //     clientIp != "2a02:5180::/32")
        // {
        //     Console.WriteLine($"Fake YooKassa notification came from invalid IP address \"{clientIp}\".");
        //     return Ok();
        // }
        if (yooKassaNotification.Event != "payment.succeeded")
        {
            Console.WriteLine($"Endpoint \"payment-confirmation/yookassa\" only processes YooKassa notifications of successful payment completion. What notification came: {yooKassaNotification.Event}.");
            return Ok();
        }

        var orderGuid = (string)yooKassaNotification.Object.metadata.OrderGuid;
        var order = await orderStorageService.Find(o => o.Guid == orderGuid);
        order.ConfirmPaymentAndBegin();
        await orderStorageService.UpdateRange(new[] { order });

        return Ok();
    }
    
    // TODO: Уверен на 90%, что это надо перенести в контроллер юзера
    [Authorize(Roles = "User")]
    [HttpGet]
    [Route("get-payment-url")]
    public async Task<IActionResult> GetPaymentUrl(string orderGuid)
    {
        var order = await orderStorageService.Find(o => o.Guid == orderGuid);
        return Ok(JsonConvert.SerializeObject(new { PaymentUrl = await orderPaymentService.GetPaymentUrl(order) }));
    }
}