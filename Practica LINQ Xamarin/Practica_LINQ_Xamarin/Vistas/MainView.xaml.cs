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
        ///  Muestra al usuario información sobre un error que él esta cometiendo
        /// </summary>
        /// <param name="mensaje"></param>
        void LanzarAdvertencia(String mensaje)
        {
            DisplayAlert("ERROR", mensaje, "OK");
        }

        void mostrarResultados(List<Contacto> contactosMostrar)
        {
            if (contactosMostrar.Count == 0)
            {
                LanzarAdvertencia("No se han encontrado resultados.");
            }
            else
            {
                lstContactos.ItemsSource = contactosMostrar;
            }
        }

        void mostrarResultados(Contacto contacto)
        {
            List<Contacto> lista = new List<Contacto>();

            if (contacto == null)
            {
                LanzarAdvertencia("No se han encontrado resultados.");
            }
            else
            {
                lista.Add(contacto);
                lstContactos.ItemsSource = lista;
            }
        }

        private bool DatosRellenados()
        {
            return !txtNombre.Text.Equals("") && !txtEdad.Text.Equals("");
        }

        #endregion Utilidades

        #region Metodos Busqueda

        private void BuscarWhere()
        {
            if (DatosRellenados())
            {
                mostrarResultados(contactos.Where(contacto => contacto.Nombre.Contains(txtNombre.Text)).ToList());
            }
            else
            {
                LanzarAdvertencia("Rellena primero los campos.");
            }

        }
        private void BuscarFirstOrDefault()
        {
            if (DatosRellenados())
            {
                mostrarResultados(contactos.Where(contacto => contacto.Nombre.Contains(txtNombre.Text)).FirstOrDefault(contacto => contacto.Edad == txtEdad.Text));
            }
            else
            {
                LanzarAdvertencia("Rellena primero los campos.");
            }
        }

        private void BuscarSingleOrDefault()
        {
            if (DatosRellenados())
            {
                mostrarResultados(contactos.Where(contacto => contacto.Nombre.Contains(txtNombre.Text)).SingleOrDefault(contacto => contacto.Edad == txtEdad.Text));
            }
            else
            {
                LanzarAdvertencia("Rellena primero los campos.");
            }
        }

        private void BuscarLastOrDefault()
        {
            if (DatosRellenados())
            {
                mostrarResultados(contactos.Where(contacto => contacto.Nombre.Contains(txtNombre.Text)).LastOrDefault(contacto => contacto.Edad == txtEdad.Text));
            }
            else
            {
                LanzarAdvertencia("Rellena primero los campos.");
            }
        }

        private void OrdenarOrderBy()
        {
            mostrarResultados(contactos.OrderBy(contacto => contacto.Nombre).ToList());
        }

        private void OrdenarOrderByDescending()
        {
            mostrarResultados(contactos.OrderByDescending(contacto => contacto.Nombre).ToList());
        }

        private void MostrarSkip()
        {
            if (DatosRellenados())
            {
                mostrarResultados(contactos.SkipWhile(contacto => Int32.Parse(contacto.Edad) > Int32.Parse(txtEdad.Text)).ToList());
            }
            else
            {
                LanzarAdvertencia("Rellena primero los campos.");
            }
        }

        private void MostrarTake()
        {
            if (DatosRellenados())
            {
                mostrarResultados(contactos.TakeWhile(contacto => Int32.Parse(contacto.Edad) > Int32.Parse(txtEdad.Text)).ToList());
            }
            else
            {
                LanzarAdvertencia("Rellena primero los campos.");
            }
        }

        #endregion Metodos Busqueda

    }
}