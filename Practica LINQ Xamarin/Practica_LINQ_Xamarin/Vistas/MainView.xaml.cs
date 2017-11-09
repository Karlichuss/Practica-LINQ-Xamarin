using Practica_LINQ_Xamarin.Modelos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Practica_LINQ_Xamarin.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainView : ContentPage
    {

        #region Variables

        List<Contacto> contactos = new List<Contacto>(); // Lista de contactos que obtendremos del fichero que el usuario debe indicar

        #endregion Variables

        #region Contructores

        public MainView()
        {
            InitializeComponent();

            /// Inicializamos los campos de texto para que no den null.
            txtNombre.Text = "";
            txtEdad.Text = "";

            /// Abrimos el archivo XML y cargamos los datos al listview
            CargarXML();
            lstContactos.ItemsSource = contactos;

            #region Eventos

            /// BOTONES - Por cada boton, hacemos una llamada al metodo correspondiente que sera un sistema de busqueda diferente cada uno.
            btnWhere.Clicked += (sender, args) =>
            {
                BuscarWhere();
            };

            btnFirstOrDefault.Clicked += (sender, args) =>
            {
                BuscarFirstOrDefault();
            };

            btnSingleOrDefault.Clicked += (sender, args) =>
            {
                BuscarSingleOrDefault();
            };

            btnLastOrDefault.Clicked += (sender, args) =>
            {
                BuscarLastOrDefault();
            };

            btnOrderBy.Clicked += (sender, args) =>
            {
                OrdenarOrderBy();
            };

            btnOrderByDescending.Clicked += (sender, args) =>
            {
                OrdenarOrderByDescending();
            };

            btnSkipWhile.Clicked += (sender, args) =>
            {
                MostrarSkip();
            };

            btnTakeWhile.Clicked += (sender, args) =>
            {
                MostrarTake();
            };

            #endregion Eventos

        }



        #endregion Constructores

        #region Metodos IO

        /// <summary>
        /// Metodo que lee el archivo XML y va construyendo los contactos y los agrega al array contactos
        /// </summary>
        private void CargarXML()
        {
            var assembly = typeof(Contacto).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("Practica_LINQ_Xamarin.Datos.Info.xml");

            // Forma propia segun la API de Xamarin
            /*using (var reader = new StreamReader(stream))
            {
                var serializer = new XmlSerializer(typeof(List<Contacto>));
                contactos = (List<Contacto>)serializer.Deserialize(reader);
            }*/

            // Alternativa propuesta por David
            StreamReader objReader = new StreamReader(stream);

            var doc = XDocument.Load(stream);

            // Forma 1 de crear la lista
            List<Contacto> contactos1 = new List<Contacto>();
            foreach (XElement element in doc.Root.Elements())
            {
                contactos.Add(new Contacto(element.Element("NOMBRE").Value, element.Element("EDAD").Value, element.Element("DNI").Value));
            }
        }

        #endregion Metodos IO

        #region Utilidades

        /// <summary>
        /// Muestra al usuario información sobre un error que él esta cometiendo
        /// </summary>
        /// <param name="mensaje">El mensaje a mostrar. Son detalles del error cometido.</param>
        void LanzarAdvertencia(String mensaje)
        {
            DisplayAlert("ERROR", mensaje, "OK");
        }

        /// <summary>
        /// Muestra el contenido del List introducido por parametro en el ListView
        /// </summary>
        /// <param name="contactosMostrar">La coleccion a mostrar</param>
        void mostrarResultados(List<Contacto> contactosMostrar)
        {
            if (contactosMostrar.Count == 0)
            {
                lstContactos.ItemsSource = null;
                LanzarAdvertencia("No se han encontrado resultados.");
            }
            else
            {
                lstContactos.ItemsSource = contactosMostrar;
            }
        }
        /// <summary>
        /// Muestra el contacto introducido por parametro en el ListView
        /// </summary>
        /// <param name="contacto">El contacto a mostrar</param>
        void mostrarResultados(Contacto contacto)
        {
            List<Contacto> lista = new List<Contacto>();

            if (contacto == null)
            {
                lstContactos.ItemsSource = null;
                LanzarAdvertencia("No se han encontrado resultados.");
            }
            else
            {
                lista.Add(contacto);
                lstContactos.ItemsSource = lista;
            }
        }

        /// <summary>
        /// Comprueba que el campo Nombre esta rellenado correctamente o no.
        /// </summary>
        /// <returns>True si el campo no esta vacio. False si esta vacio.</returns>
        private bool DatosRellenados()
        {
            return !txtNombre.Text.Equals("");
        }

        /// <summary>
        /// Comprueba que el campo Edad esta rellenado correctamente o no.
        /// </summary>
        /// <returns>True si hay introducido un numero. False si esta vacio o no es un numero.</returns>
        private bool ControlNumerico()
        {
            return Int32.TryParse(txtEdad.Text, out int edad);
        }

        #endregion Utilidades

        #region Metodos Busqueda

        /// <summary>
        /// Fitra por nombre
        /// </summary>
        private void BuscarWhere()
        {
            if (DatosRellenados())
            {
                mostrarResultados(contactos.Where(contacto => contacto.Nombre.ToLower().Contains(txtNombre.Text.ToLower())).ToList());
            }
            else
            {
                LanzarAdvertencia("Rellena primero el campo Nombre.");
            }

        }

        /// <summary>
        /// Filtra por edad
        /// </summary>
        private void BuscarFirstOrDefault()
        {
            if (ControlNumerico())
            {
                mostrarResultados(contactos.FirstOrDefault(contacto => contacto.Edad == txtEdad.Text));
            }
            else
            {
                LanzarAdvertencia("Rellena primero el campo Edad.");
            }
        }

        /// <summary>
        /// Filtra por nombre y por edad
        /// </summary>
        private void BuscarSingleOrDefault()
        {
            if (DatosRellenados() && ControlNumerico())
            {
                mostrarResultados(contactos.Where(contacto => contacto.Nombre.ToLower().Contains(txtNombre.Text.ToLower())).SingleOrDefault(contacto => contacto.Edad == txtEdad.Text));
            }
            else
            {
                LanzarAdvertencia("Rellena primero los campos correctamente.");
            }
        }

        /// <summary>
        /// Filtra por edad
        /// </summary>
        private void BuscarLastOrDefault()
        {
            if (ControlNumerico())
            {
                mostrarResultados(contactos.LastOrDefault(contacto => contacto.Edad == txtEdad.Text));
            }
            else
            {
                LanzarAdvertencia("Rellena primero el campo Edad.");
            }
        }

        /// <summary>
        /// Ordena por orden alfabetico
        /// </summary>
        private void OrdenarOrderBy()
        {
            mostrarResultados(contactos.OrderBy(contacto => contacto.Nombre.ToLower()).ToList());
        }

        /// <summary>
        /// Ordena por orden alfabetico inverso
        /// </summary>
        private void OrdenarOrderByDescending()
        {
            mostrarResultados(contactos.OrderByDescending(contacto => contacto.Nombre.ToLower()).ToList());
        }

        /// <summary>
        /// Muestra los resultados a partir de la primera coincidencia que encuentra que sea mayor que la edad de txtEdad
        /// </summary>
        private void MostrarSkip()
        {
            if (ControlNumerico())
            {
                mostrarResultados(contactos.SkipWhile(contacto => Int32.Parse(contacto.Edad) > Int32.Parse(txtEdad.Text)).ToList());
            }
            else
            {
                LanzarAdvertencia("Rellena primero el campo de Edad.");
            }
        }

        /// <summary>
        /// Muestra los resultados hasta la primera coincidencia que encuentra que sea mayor que la edad de txtEdad
        /// </summary>
        private void MostrarTake()
        {
            if (ControlNumerico())
            {
                mostrarResultados(contactos.TakeWhile(contacto => Int32.Parse(contacto.Edad) > Int32.Parse(txtEdad.Text)).ToList());
            }
            else
            {
                LanzarAdvertencia("Rellena primero el campo de Edad");
            }
        }

        #endregion Metodos Busqueda

    }
}