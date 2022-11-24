using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;

namespace Play.Catalog.Service.Controllers
{
    //https://localhost:5001/items
    [ApiController]
    [Route("items")]
    public class ItemsController: ControllerBase
    {
        private static readonly List<ItemDto> items = new List<ItemDto>
        {
            new ItemDto(Guid.NewGuid(),"Potion","Restores a small amount of HP",5,DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(),"Antidote","Cures poison",7,DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(),"Bronze sword","Deals a small amount of damage",20,DateTimeOffset.UtcNow),
        };

        [HttpGet]
        public IEnumerable<ItemDto> Get()
        {
            return items;
        }
        // GET /items/1234
        [HttpGet("{id}")]
        public ActionResult<ItemDto> GetById(Guid id){
            var item = items.Where(item => item.Id ==id).SingleOrDefault();
            if(item == null){
                return NotFound();
            }
            return item;
        }

        [HttpPost]
        public ActionResult<ItemDto> Post(CreateItemDto createItemDto)
        {
            var item = new ItemDto(Guid.NewGuid(),createItemDto.Name,createItemDto.Description,createItemDto.Price,DateTimeOffset.UtcNow);
            items.Add(item);

            return CreatedAtAction(nameof(GetById),new {id=item.Id},item);
        }
        // PUT /items/1234
        [HttpPut("{id}")]
        public IActionResult Put(Guid id,UpdateItemDto updateItemDto)
        {
            var existingItem = items.Where(items => items.Id == id).SingleOrDefault();
            if(existingItem == null){
                return NotFound();
            }
            var updateItem = existingItem with{
                Name = updateItemDto.Name,
                Description = updateItemDto.Description,
                Price = updateItemDto.Price
            };

            var index = items.FindIndex(existingItem => existingItem.Id == id);
            items[index] = updateItem;

            return NoContent();
        }

        [HttpDelete("{id}")]

        public IActionResult Delete(Guid id){
            var item = items.FindIndex(it => it.Id ==id);
            if(item < 0){
                return NotFound();
            }
            items.RemoveAt(item);

            return NoContent();
        }
    }

}