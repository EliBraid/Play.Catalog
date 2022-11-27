using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Repositories;
using Play.Common;

namespace Play.Catalog.Service.Controllers
{
    //https://localhost:5001/items
    [ApiController]
    [Route("items")]
    public class ItemsController: ControllerBase
    {
        private readonly IRepository<Item> itemRepository;

        private static int requestCounter = 0;
        public ItemsController(IRepository<Item> itemRepository)
        {
            this.itemRepository = itemRepository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
        {
            requestCounter++;
            System.Console.WriteLine($"Request {requestCounter} : Starting ...");
            if(requestCounter <= 2){
                System.Console.WriteLine($"Request {requestCounter} : Delaying ...");
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
            if(requestCounter <= 4){
                System.Console.WriteLine($"Request {requestCounter} : 500 (Internal Server Error).");
                return StatusCode(500);
            }
            var items = (await itemRepository.GetAllAsync()).Select(item=>
            item.AsDto());
            System.Console.WriteLine($"Request {requestCounter} : 200 (OK).");
            return Ok(items);
        }
        // GET /items/1234
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id){
            var item = await itemRepository.GetAsync(id);
            if(item == null){
                return NotFound();
            }
            return item.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
        {
           var item = new Item{
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreateTime = DateTimeOffset.UtcNow
           };
           await itemRepository.CreateAsync(item);

            return CreatedAtAction(nameof(GetByIdAsync),new {id=item.Id},item);
        }
        // PUT /items/1234
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id,UpdateItemDto updateItemDto)
        {
            var existingItem = await itemRepository.GetAsync(id);
            if(existingItem==null){
                return NotFound();
            }
            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;

            await itemRepository.UpdateAsync(existingItem);

            return NoContent();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteAsync(Guid id){
            var item = await itemRepository.GetAsync(id);
            if(item==null){
              return  NotFound();
            }
            await itemRepository.RemoveAsync(item.Id);

            return NoContent();
        }
    }

}