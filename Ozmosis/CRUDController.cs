using System.Linq;
using System.Web.Mvc;

namespace Ozmosis
{
    class CRUDController<TEntity> : Controller where TEntity : class, IEntity, new()
    {
        [Microsoft.Practices.Unity.Dependency]
        public IRepository Repository { get; set; }

        public ActionResult Index()
        {
            var entities = Repository.GetAll<TEntity>().ToList();
            return View(entities);
        }

        public ActionResult Details(int id)
        {
            var entity = Repository.Get<TEntity>(id);
            return View(entity);
        }

        public ActionResult Edit(int id)
        {
            var entity = Repository.Get<TEntity>(id);
            return View(entity);
        }

        [HttpPost, UnitOfWork]
        public ActionResult Edit(int id, FormCollection values)
        {
            var entity = Repository.Get<TEntity>(id);
            if (TryUpdateModel<TEntity>(entity))
                return RedirectToAction("index", new { id = entity.Id });
            return View(entity);
        }

        public ActionResult Create()
        {
            return View(new TEntity());
        }

        [HttpPost, UnitOfWork]
        public ActionResult Create(TEntity entity)
        {
            if (ModelState.IsValid)
            {
                Repository.Add(entity);
                return RedirectToAction("index");            
            }
            return View(entity);
        }
    }
}