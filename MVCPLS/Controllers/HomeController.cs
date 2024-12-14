using System;
using System.Web.Mvc;
using BLL;
using Entities;
using Seguridad;

namespace YourNamespace.Controllers
{
    public class HomeController : Controller
    {
        private ProductsLogic _logic = new ProductsLogic();
        private CategoriesLogic _categoryLogic = new CategoriesLogic();
        private UserService _userService = new UserService(new SecurityDBEntities());

        // Verificar si el usuario está autenticado
        private bool UserIsAuthenticated()
        {
            if (Session["UserId"] != null)
            {
                int userId = (int)Session["UserId"];
                var user = _userService.GetUserById(userId);
                return user != null;
            }
            return false;
        }

        // Obtener el usuario logueado
        private Users GetLoggedUser()
        {
            if (Session["UserId"] != null)
            {
                int userId = (int)Session["UserId"];
                return _userService.GetUserById(userId);
            }
            return null;
        }

        // Cerrar sesión
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // Verificar si el usuario es administrador
        private bool UserIsAdmin()
        {
            if (Session["UserId"] != null)
            {
                int userId = (int)Session["UserId"];
                var user = _userService.GetUserById(userId);
                return user?.Role == "Admin";
            }
            return false;
        }

        // Redirigir al login si no está autenticado
        private ActionResult RedirectToLoginIfNotAuthenticated()
        {
            if (!UserIsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }
            return null;
        }

        // Página de inicio
        public ActionResult Index()
        {
            var redirectResult = RedirectToLoginIfNotAuthenticated();
            if (redirectResult != null)
            {
                return redirectResult;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(int id, string searchType)
        {
            var redirectResult = RedirectToLoginIfNotAuthenticated();
            if (redirectResult != null)
            {
                return redirectResult;
            }

            try
            {
                if (searchType == "product")
                {
                    var product = _logic.RetrieveById(id);
                    if (product == null)
                    {
                        ViewBag.ErrorMessage = "Producto no encontrado.";
                        return View();
                    }
                    return View("Details", product);
                }
                else if (searchType == "category")
                {
                    var category = _categoryLogic.RetrieveById(id);
                    if (category == null)
                    {
                        ViewBag.ErrorMessage = "Categoría no encontrada.";
                        return View();
                    }
                    return View("CategoryDetails", category);
                }
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }

        // Detalles de producto
        public ActionResult Details(int id)
        {
            var redirectResult = RedirectToLoginIfNotAuthenticated();
            if (redirectResult != null)
            {
                return redirectResult;
            }

            var product = _logic.RetrieveById(id);
            if (product == null)
            {
                ViewBag.ErrorMessage = "Producto no encontrado.";
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // Formulario CUD para productos
        public ActionResult CUD(int? id = null)
        {
            var redirectResult = RedirectToLoginIfNotAuthenticated();
            if (redirectResult != null)
            {
                return redirectResult;
            }

            if (!UserIsAdmin())
            {
                return RedirectToAction("Index");
            }

            Products model = id.HasValue ? _logic.RetrieveById(id.Value) : new Products();
            if (model == null && id.HasValue)
            {
                ViewBag.ErrorMessage = "Producto no encontrado.";
                return RedirectToAction("Index");
            }

            ViewBag.Categories = new SelectList(_categoryLogic.RetrieveAll(), "CategoryID", "CategoryName", model.CategoryID);
            return View(model);
        }

        [HttpPost]
        public ActionResult CUD(Products model, string CreateBtn, string UpdateBtn, string DeleteBtn)
        {
            var redirectResult = RedirectToLoginIfNotAuthenticated();
            if (redirectResult != null)
            {
                return redirectResult;
            }

            if (!UserIsAdmin())
            {
                return RedirectToAction("Index");
            }

            try
            {
                var loggedUser = GetLoggedUser();
                var username = loggedUser?.Username ?? "Desconocido";

                if (!string.IsNullOrEmpty(CreateBtn))
                {
                    _logic.Create(model);
                    LogService.LogAction(username, $"Creó el producto con ID {model.ProductID}.");
                    ViewBag.SuccessMessage = "Producto creado exitosamente.";
                    return RedirectToAction("Details", new { id = model.ProductID });
                }
                else if (!string.IsNullOrEmpty(UpdateBtn))
                {
                    _logic.Update(model);
                    LogService.LogAction(username, $"Actualizó el producto con ID {model.ProductID}.");
                    ViewBag.SuccessMessage = "Producto actualizado exitosamente.";
                    return RedirectToAction("Details", new { id = model.ProductID });
                }
                else if (!string.IsNullOrEmpty(DeleteBtn))
                {
                    if (_logic.Delete(model.ProductID))
                    {
                        LogService.LogAction(username, $"Eliminó el producto con ID {model.ProductID}.");
                        ViewBag.SuccessMessage = "Producto eliminado exitosamente.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "No se puede eliminar el producto. Asegúrate de que las unidades en existencia sean 0.";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View(model);
        }

        // Formulario CUD para categorías
        public ActionResult CategoryCUD(int? id = null)
        {
            var redirectResult = RedirectToLoginIfNotAuthenticated();
            if (redirectResult != null)
            {
                return redirectResult;
            }

            if (!UserIsAdmin())
            {
                return RedirectToAction("Index");
            }

            Categories model = id.HasValue ? _categoryLogic.RetrieveById(id.Value) : new Categories();
            if (model == null && id.HasValue)
            {
                ViewBag.ErrorMessage = "Categoría no encontrada.";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult CategoryCUD(Categories model, string CreateBtn, string UpdateBtn, string DeleteBtn)
        {
            var redirectResult = RedirectToLoginIfNotAuthenticated();
            if (redirectResult != null)
            {
                return redirectResult;
            }

            if (!UserIsAdmin())
            {
                return RedirectToAction("Index");
            }

            try
            {
                var loggedUser = GetLoggedUser();
                var username = loggedUser?.Username ?? "Desconocido";

                if (!string.IsNullOrEmpty(CreateBtn))
                {
                    _categoryLogic.Create(model);
                    LogService.LogAction(username, $"Creó la categoría con ID {model.CategoryID}.");
                    ViewBag.SuccessMessage = "Categoría creada exitosamente.";
                    return RedirectToAction("CategoryDetails", new { id = model.CategoryID });
                }
                else if (!string.IsNullOrEmpty(UpdateBtn))
                {
                    _categoryLogic.Update(model);
                    LogService.LogAction(username, $"Actualizó la categoría con ID {model.CategoryID}.");
                    ViewBag.SuccessMessage = "Categoría actualizada exitosamente.";
                    return RedirectToAction("CategoryDetails", new { id = model.CategoryID });
                }
                else if (!string.IsNullOrEmpty(DeleteBtn))
                {
                    if (_categoryLogic.Delete(model.CategoryID))
                    {
                        LogService.LogAction(username, $"Eliminó la categoría con ID {model.CategoryID}.");
                        ViewBag.SuccessMessage = "Categoría eliminada exitosamente.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "No se puede eliminar la categoría.";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View(model);
        }

        // Acción para mostrar los detalles de una categoría
        public ActionResult CategoryDetails(int id)
        {
            var redirectResult = RedirectToLoginIfNotAuthenticated();
            if (redirectResult != null)
            {
                return redirectResult;
            }

            var category = _categoryLogic.RetrieveById(id);
            if (category == null)
            {
                ViewBag.ErrorMessage = "Categoría no encontrada.";
                return RedirectToAction("Index");
            }
            return View(category);
        }
    }
}
