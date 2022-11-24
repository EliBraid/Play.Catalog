using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Repositories;

namespace Play.Catalog.Service.Controllers
{
    //https://localhost:5001/items
    [ApiController]
    [Route("items")]
    public class ItemsController: ControllerBase
    {
        private readonly ItemRepository itemRepository = new();

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetAsync()
        {
            var items = (await itemRepository.GetAllAsync()).Select(item=>
            item.AsDto());
            return items;
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