using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Logica_IconMaker;


namespace IconMaker_1._0
{
    public partial class form_IconMaker : Form
    {
        public enum herramientas { pintar, rellenar, borrar, seleccColor, seleccionar, finseleccionar, pegar,ninguna}; //Enum que representan los diferentes tipo de herramientas que se pueden seleccionar
        herramientas herrSeleccionada;  //Tiene el valor de la herramienta seleccionada en cada momento

        Point inicioSeleccion;  //Puntos que indican el inicio y final de la seleccion
        Point finalSeleccion;
        bool seleccionando;     //Cuando esta true significa que se esta realizando una seleccion
        Pen rejilla;    //Pluma que tiene el color de las regillas de picturebox_HojaTrabajo
        NuevoIcono nuevoIco;   //Instancia del form NuevoIcono
        string nombreSeleccionado;  //me da el nombre del picturebox (herramienta) que tengo activa (solo se usa para modifica el fondo de los picturebox herramientas)
        Bitmap bit; //bitmap que se usa para la vistaprevia y para guardar


        private int dimensionXX;    //representa la dimension en pixeles del icono

        bool mouseClick; //se usa para en el mousemove para saber si el click esta presionado
        public HojadeTrabajo hojaTrab;  //instancia de la la Logica
        public form_IconMaker()
        {
            dimensionXX = 16;   //Tamaño por defecto
            InitializeComponent();  
            hojaTrab = new HojadeTrabajo(dimensionXX);  //LLamando constructor de la Logica
            ArreglaHojaTrabajo();   
            herrSeleccionada = herramientas.pintar; //Inicializando por defecto la herramienta pintar
            nombreSeleccionado = "pictureBox_Pintar";   
            mouseClick = false;
            seleccionando = false;
            rejilla = new Pen(panel_ColorRejilla.BackColor);    //Por defecto el color de la regilla se pone negro            
            pictureBox_Pintar.BackColor = Color.Purple; //cambiando el fondo de pbx pintar para que el usuario vea que tiene seleccionado
            pictureBox_HojaTrabajo.Cursor = new Cursor(Properties.Resources.curPintar.Handle);  //Cambia el cursor del pbx HojaTrabajo, y pone el correspondiente con su herramienta            
            bit = new Bitmap(dimensionXX, dimensionXX); //Inicializa el bitmap
        }
        
        private void form_IconMaker_Load(object sender, EventArgs e)
        {
            //Seleccion del los 1ros valores de cada comboBox
            comboBox_Patron.SelectedIndex = 0;
            comboBox_Zoom.SelectedIndex = 0;
        }

        
        #region Pintar PBX

        private void pictureBox_HojaTrabajo_Paint(object sender, PaintEventArgs e)
        {
            int y = pictureBox_HojaTrabajo.Height;
            int x = pictureBox_HojaTrabajo.Width;

            int cellSize = (pictureBox_HojaTrabajo.Width / dimensionXX); //ancho de cada casilla

            for (int i = 0; i < dimensionXX; i++)
            {
                for (int j = 0; j < dimensionXX; j++)
                {
                    SolidBrush b = new SolidBrush(hojaTrab[i, j]);
                    e.Graphics.FillRectangle(b, i * (x / dimensionXX), j * (y / dimensionXX), (x / dimensionXX), (y / dimensionXX)); //segun lo que tenga HojaTrabajo se pinta el pbx Rellenando un Rectangulo
                    if (checkBox_Rejillas.Checked)  // Si el checkbox rejillas esta activo se pintan las lineas
                    {
                        e.Graphics.DrawLine(rejilla, i * cellSize, j * cellSize, i * cellSize, j * cellSize + cellSize);
                        e.Graphics.DrawLine(rejilla, i * cellSize, j * cellSize, i * cellSize + cellSize, j * cellSize);
                    }
                }
            }

            if (herrSeleccionada == herramientas.finseleccionar) //en caso de que se este seleccionando se dibuja un rectangulo que encierra dicha seleccion
            {
                Point tempPoint1 = new Point();
                Point tempPoint2 = new Point();
                int tamañoCelda = pictureBox_HojaTrabajo.Width / dimensionXX;
                Pen p = new Pen(Color.SteelBlue, 6);

                //se realiza un filtrado y actualizacion de las var tempPoints para poder pintar bien en cada una de las cuatro direcciones posibles de seleccion
                if (inicioSeleccion.X > finalSeleccion.X)
                {
                    tempPoint1.X = finalSeleccion.X;
                    tempPoint2.X = inicioSeleccion.X;
                    if (inicioSeleccion.Y > finalSeleccion.Y)
                    {
                        tempPoint1.Y = finalSeleccion.Y;
                        tempPoint2.Y = inicioSeleccion.Y;
                    }
                    else
                    {
                        tempPoint1.Y = inicioSeleccion.Y;
                        tempPoint2.Y = finalSeleccion.Y;
                    }
                }
                else
                {
                    tempPoint1.X = inicioSeleccion.X;
                    tempPoint2.X = finalSeleccion.X;
                    if (inicioSeleccion.Y > finalSeleccion.Y)
                    {
                        tempPoint1.Y = finalSeleccion.Y;
                        tempPoint2.Y = inicioSeleccion.Y;
                    }
                    else
                    {
                        tempPoint1.Y = inicioSeleccion.Y;
                        tempPoint2.Y = finalSeleccion.Y;
                    }
                }
                e.Graphics.DrawRectangle(p, tempPoint1.X * tamañoCelda, tempPoint1.Y * tamañoCelda, ((tempPoint2.X - tempPoint1.X) + 1) * tamañoCelda, ((tempPoint2.Y - tempPoint1.Y) + 1) * tamañoCelda);
            }
        }

        private void pictureBox_VistaPrevia_Paint(object sender, PaintEventArgs e)
        {
            Actualiza();
            //pinta una imagen usando el bitmap bit
            e.Graphics.DrawImage(bit, 0, 0, 67, 67);            
        }

        private void Actualiza()
        {
            //recorre HojaTrabajo y actualiza bit
            bit = new Bitmap(dimensionXX, dimensionXX);
            for (int i = 0; i < dimensionXX; i++)
            {
                for (int j = 0; j < dimensionXX; j++)
                {
                    bit.SetPixel(i, j, hojaTrab[i, j]);
                }
            }
        }

        #endregion


        #region Herramientas (Acciones con Mouse)

        private void Herramientas_MouseMoveOn(object sender, MouseEventArgs e)
        {
            ((PictureBox)sender).BackColor = Color.SteelBlue; //cambia el color fondo del pbx que manda el evento
        }

        private void Herramientas_MouseLeave(object sender, EventArgs e)
        {
            if (((PictureBox)sender).Name != nombreSeleccionado) //si nombre del pbx no el que esta seleccionado, poner el fondo trasparente, de lo contrario lo pone morado
                ((PictureBox)sender).BackColor = Color.Empty;
            else
                ((PictureBox)sender).BackColor = Color.Purple;
        }

        private void Herramientas_MouseClick(object sender, EventArgs e)
        {
            if (herrSeleccionada == herramientas.finseleccionar && !seleccionando) //evalua si se termino de hacer una seleccion
            {
                //actualiza la herramiento seleccionada, el fondo del pbx Seleccionar, y repinta pbx HojaTrabajo
                herrSeleccionada = herramientas.ninguna;
                pictureBox_Seleccionar.BackColor = Color.Empty;
                pictureBox_HojaTrabajo.Invalidate();
            }

            //Filtra el por el nombre del pbx sender para escoger la herramienta correspondiente
            switch (((PictureBox)sender).Name)
            {
                case ("pictureBox_Pintar"):
                    herrSeleccionada = herramientas.pintar;
                    pictureBox_HojaTrabajo.Cursor = new Cursor(Properties.Resources.curPintar.Handle);
                    break;
                case ("pictureBox_Rellenar"):
                    herrSeleccionada = herramientas.rellenar;
                    pictureBox_HojaTrabajo.Cursor = new Cursor(Properties.Resources.curRellenar.Handle);
                    break;
                case ("pictureBox_Borrar"):
                    herrSeleccionada = herramientas.borrar;
                    pictureBox_HojaTrabajo.Cursor = new Cursor(Properties.Resources.curBorrar.Handle);
                    break;
                case ("pictureBox_SeleccionarColor"):
                    herrSeleccionada = herramientas.seleccColor;
                    pictureBox_HojaTrabajo.Cursor = new Cursor(Properties.Resources.curSelecColor.Handle);
                    break;
                case ("pictureBox_Seleccionar"):
                    pictureBox_HojaTrabajo.Cursor = Cursors.Cross;
                    herrSeleccionada = herramientas.seleccionar;
                    break;
                case ("pictureBox_Cortar"):
                    herrSeleccionada = herramientas.ninguna;
                    pictureBox_HojaTrabajo.Cursor = Cursors.Default;
                    hojaTrab.Cortar();
                    pictureBox_HojaTrabajo.Invalidate();
                    pictureBox_VistaPrevia.Invalidate();
                    break;
                case ("pictureBox_Copiar"):
                    herrSeleccionada = herramientas.ninguna;
                    pictureBox_HojaTrabajo.Cursor = Cursors.Default;
                    hojaTrab.Copiar();
                    break;
                case ("pictureBox_Pegar"):
                    pictureBox_HojaTrabajo.Cursor = Cursors.Default;
                    herrSeleccionada = herramientas.pegar;
                    break;
                default:
                    break;
            }
            nombreSeleccionado = ((PictureBox)sender).Name; //actualiza nombreSeleccionado para ponerle fondo al pbx de la herramienta correspondiente
            if (herrSeleccionada != herramientas.pintar)
                comboBox_Patron.Enabled = false;
            else
                comboBox_Patron.Enabled = true;
            if (herrSeleccionada != herramientas.pintar && herrSeleccionada != herramientas.borrar)
                textBox_Grosor.Enabled = false;
            else
                textBox_Grosor.Enabled = true;
            
            FondoPBX((PictureBox)sender); //pone fondo del pbx seleccionado de color morado
        }

        //      //      //      //      //      //      //      //      //      //

        private void pictureBox_CambioColor_Click(object sender, EventArgs e) //hace un swap entre el color principal y el secundario
        {
            Color temp = panel_ColorSelec1.BackColor;
            panel_ColorSelec1.BackColor = panel_ColorSelec2.BackColor;
            panel_ColorSelec2.BackColor = temp;
        }

        private void pictureBox_Deshacer_Click(object sender, EventArgs e)
        {
            hojaTrab.Deshacer();
            pictureBox_HojaTrabajo.Invalidate();
            pictureBox_VistaPrevia.Invalidate();
        } //llama al metodo rehacer/deshacer de la Logica

        private void pictureBox_Rehacer_Click(object sender, EventArgs e)
        {
            hojaTrab.Rehacer();
            pictureBox_HojaTrabajo.Invalidate();
            pictureBox_VistaPrevia.Invalidate();
        }        

        #endregion


        #region Menu (ToolStripMenu)

        private void nuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            nuevoIco = new NuevoIcono(); 
            nuevoIco.principalFormulario = this; //Le asignamos a principalFormulario en el form nuevoIco el valor del form_HojaTrabajo
            nuevoIco.ShowDialog();
        }

        private void cargarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialogLoad.ShowDialog() == DialogResult.OK)
            {
                Bitmap b = new Bitmap(openFileDialogLoad.FileName); //bitmap temp que carga el icono en la direccion que da el openFileDialog
                dimensionXX = b.Width;
                hojaTrab = new HojadeTrabajo(dimensionXX);  //se crea una nueva HojaTrabajo con la dimension del bitmap b
                for (int i = 0; i < b.Height; i++)
                {
                    for (int j = 0; j < b.Width; j++)
                    {
                        hojaTrab[i, j] = b.GetPixel(i, j);  //se actualiza hojaTraba con los valores dle bitmap
                    }
                }
                //Se actulizann todos los controles del formulario, cursor,herrmientaSeleccionada, label_Dimension ...
                ArreglaHojaTrabajo();
                pictureBox_HojaTrabajo.Invalidate();
                pictureBox_VistaPrevia.Invalidate();
                herrSeleccionada = herramientas.pintar;
                pictureBox_HojaTrabajo.Cursor = new Cursor(Properties.Resources.curPintar.Handle);
                FondoPBX(pictureBox_Pintar);
                label_DimActual.Text = dimensionXX + " x " + dimensionXX + " Píxeles";
                comboBox_Zoom.SelectedIndex = 0;
                Zoom();
            }

        }

        private void guardarToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                bit.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);   
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void copiarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            herrSeleccionada = herramientas.ninguna;
            pictureBox_HojaTrabajo.Cursor = Cursors.Default;
            hojaTrab.Copiar();
        }

        private void cortarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            herrSeleccionada = herramientas.ninguna;
            pictureBox_HojaTrabajo.Cursor = Cursors.Default;
            hojaTrab.Cortar();
            pictureBox_HojaTrabajo.Invalidate();
            pictureBox_VistaPrevia.Invalidate();

        }

        private void pegarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox_HojaTrabajo.Cursor = Cursors.Default;
            herrSeleccionada = herramientas.pegar;
        }

        private void deshacerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hojaTrab.Deshacer();
            pictureBox_HojaTrabajo.Invalidate();
            pictureBox_VistaPrevia.Invalidate();
        }

        private void rehacerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hojaTrab.Rehacer();
            pictureBox_HojaTrabajo.Invalidate();
            pictureBox_VistaPrevia.Invalidate();
        }

        private void acercaDeIconMakerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AcercaDe fomrAcercaDe = new AcercaDe();
            fomrAcercaDe.ShowDialog();
        }

        #endregion


        #region Acciones sobre PBX HojaTrabajo (Lienzo)

        private void pictureBox_HojaTrabajo_MouseClick(object sender, MouseEventArgs e)
        {
            if (herrSeleccionada == herramientas.finseleccionar && !seleccionando) //Evalua si no se estaba realizando una seleccion
            {
                herrSeleccionada = herramientas.ninguna;
                pictureBox_HojaTrabajo.Cursor = Cursors.Default;
                pictureBox_Seleccionar.BackColor = Color.Empty;
            }
            hojaTrab.Limpia();  //Quita en la logica los valores de los puntos inicio, final y selecciona
            int i = e.X * dimensionXX / pictureBox_HojaTrabajo.Width;
            int j = e.Y * dimensionXX / pictureBox_HojaTrabajo.Height;
            switch (e.Button)   //Escoge que boton del mouse se esta haciendo referencia
            {
                case MouseButtons.Left:
                    switch (herrSeleccionada)
                    {
                        case herramientas.pintar:
                            //llamada a Dibuja pasandole, grosor,color del panel colorSelecinado, y un strign patron que lo da el comboBox_Patron
                            hojaTrab.Dibuja(i, j, int.Parse(textBox_Grosor.Text) - 1, panel_ColorSelec1.BackColor, comboBox_Patron.Text);
                            break;
                        case herramientas.rellenar:
                            hojaTrab.Rellena(new Point(i, j), panel_ColorSelec1.BackColor);
                            break;
                        case herramientas.borrar:
                            hojaTrab.Dibuja(i, j, int.Parse(textBox_Grosor.Text) - 1, Color.Empty, "Cuadrado");
                            break;
                        case herramientas.seleccColor:
                            panel_ColorSelec1.BackColor = hojaTrab[i, j];
                            break;
                        case herramientas.seleccionar:
                            inicioSeleccion = new Point(i, j);
                            seleccionando = true;
                            herrSeleccionada = herramientas.finseleccionar;
                            break;
                        case herramientas.pegar:
                            hojaTrab.Pegar(new Point(i, j));
                            break;
                        default:                                                        
                            break;
                    }
                    break;

                case MouseButtons.Right:
                    switch (herrSeleccionada)
                    {
                        case herramientas.pintar:
                            hojaTrab.Dibuja(i, j, int.Parse(textBox_Grosor.Text) - 1, panel_ColorSelec2.BackColor, comboBox_Patron.Text);
                            break;
                        case herramientas.rellenar:
                            hojaTrab.Rellena(new Point(i, j), panel_ColorSelec2.BackColor);
                            break;
                        case herramientas.borrar:
                            hojaTrab[i, j] = Color.Empty;
                            break;
                        case herramientas.seleccColor:
                            panel_ColorSelec2.BackColor = hojaTrab[i, j];
                            break;
                        default:
                            break;
                    }
                    break;
            }

            pictureBox_HojaTrabajo.Invalidate();
            pictureBox_VistaPrevia.Invalidate();
        }

        private void pictureBox_HojaTrabajo_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseClick) //evalua el click esta precionado
            {
                int i = e.X * dimensionXX / pictureBox_HojaTrabajo.Width;
                int j = e.Y * dimensionXX / pictureBox_HojaTrabajo.Height;
                if (hojaTrab.EstaEnRango(i, j)) // si esta dentro de los rangos del picturebox_hojaTrabajo llama pictureBox_HojaTrabajo_MouseClick
                {
                    pictureBox_HojaTrabajo_MouseClick(sender, e);
                    if (herrSeleccionada == herramientas.finseleccionar)
                        finalSeleccion = new Point(e.X * dimensionXX / pictureBox_HojaTrabajo.Width, e.Y * dimensionXX / pictureBox_HojaTrabajo.Height);
                }
            }
        }

        private void pictureBox_HojaTrabajo_MouseDown(object sender, MouseEventArgs e)
        {
            //cuando se realiza el eveneto mousedown se pone en true la variable mouseClick
            mouseClick = true;
        }

        private void pictureBox_HojaTrabajo_MouseUp(object sender, MouseEventArgs e)
        {
            mouseClick = false; //deja de hacer click
            if (herrSeleccionada == herramientas.finseleccionar) //cuando se esta haciendo una seleccion se le pasa al metodo Seleccion de la logica
                //el punto inicio y el punto done se realizo el MouseUp
            {
                finalSeleccion = new Point(e.X * dimensionXX / pictureBox_HojaTrabajo.Width, e.Y * dimensionXX / pictureBox_HojaTrabajo.Height); 
                hojaTrab.Seleccion(inicioSeleccion, finalSeleccion);
                seleccionando = false;
            }
            if (herrSeleccionada != herramientas.ninguna) //cuando la herramienta seleccionada es diferente a ninguna, entoces se realizo algun cambio
                //llama a modificacion, que guarda el valor de la matriz principal de la logica, y vacia la pila rehacer
            {
                hojaTrab.Modificacion();
                hojaTrab.VaciaRehacer();
            }
        }

        #endregion
        

        #region Eventos Controles

        private void checkBox_Rejillas_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_Rejillas.Checked) //activa o decativa las rejillas del pbx hojaTrabajo y las pones del color seleccionado
            {
                rejilla = new Pen(panel_ColorRejilla.BackColor);
                pictureBox_HojaTrabajo.Invalidate();

            }
            else
            {
                rejilla = new Pen(Color.Empty);
                pictureBox_HojaTrabajo.Invalidate();
            }
        }

        private void textBox_Grosor_TextChanged(object sender, EventArgs e) //restringe al usuario a pasar solo numero, y que esten entre 1 y 99
        {
            try
            {
                int.Parse(textBox_Grosor.Text);
                if (int.Parse(textBox_Grosor.Text) < 1)
                {
                    textBox_Grosor.Text = "1";
                    System.Windows.Forms.MessageBox.Show("Solo valores entre 1 y 99", "IconMaker 1.0", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (int.Parse(textBox_Grosor.Text) > 99)
                {
                    textBox_Grosor.Text = "99";
                    System.Windows.Forms.MessageBox.Show("Solo valores entre 1 y 99", "IconMaker 1.0", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch
            {
                textBox_Grosor.Text = "1";
                System.Windows.Forms.MessageBox.Show("Solo valores entre 1 y 99", "IconMaker 1.0", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void form_IconMaker_FormClosing(object sender, FormClosingEventArgs e) // pregunta al usuario si desea guardar el icono que esta haciendo
        {
            DialogResult res;
            res = MessageBox.Show("¿Desea guardar los cambios?", "IconMaker 1.0", MessageBoxButtons.YesNoCancel);
            if (res == DialogResult.No)
                e.Cancel = false;
            else if (res == DialogResult.Cancel)
                e.Cancel = true;
            else
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    bit.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Icon);
                    e.Cancel = false;
                }
            }
        }

        private void comboBox_Zoom_SelectedValueChanged(object sender, EventArgs e)
        {
            Zoom();
        }

        private void Zoom() // mmodifica la dimensio del pbx hojaTrabajo
        {
            int zoomSeleccionado = int.Parse(comboBox_Zoom.Text.Split('x')[0]);
            switch (dimensionXX)
            {
                case (64):
                    {
                        if (zoomSeleccionado == 1)
                        {
                            pictureBox_HojaTrabajo.Width = panel_HojaTrabajo.Width;
                            pictureBox_HojaTrabajo.Height = panel_HojaTrabajo.Height;
                            pictureBox_HojaTrabajo.Invalidate();
                        }
                        else
                        {
                            pictureBox_HojaTrabajo.Width = panel_HojaTrabajo.Width * (zoomSeleccionado / 2 + 1);
                            pictureBox_HojaTrabajo.Height = panel_HojaTrabajo.Height * (zoomSeleccionado / 2 + 1);
                            pictureBox_HojaTrabajo.Invalidate();
                        }
                    }
                    break;

                case (128):
                    {
                        if (zoomSeleccionado == 1)
                        {
                            pictureBox_HojaTrabajo.Width = panel_HojaTrabajo.Width * 2;
                            pictureBox_HojaTrabajo.Height = panel_HojaTrabajo.Height * 2;
                            pictureBox_HojaTrabajo.Invalidate();
                        }
                        else
                        {
                            pictureBox_HojaTrabajo.Width = panel_HojaTrabajo.Width * (zoomSeleccionado / 2 + 2);
                            pictureBox_HojaTrabajo.Height = panel_HojaTrabajo.Height * (zoomSeleccionado / 2 + 2);
                            pictureBox_HojaTrabajo.Invalidate();
                        }
                    }
                    break;
                case (256):
                    {
                        if (zoomSeleccionado == 1)
                        {
                            pictureBox_HojaTrabajo.Width = panel_HojaTrabajo.Width * 4;
                            pictureBox_HojaTrabajo.Height = panel_HojaTrabajo.Height * 4;
                            pictureBox_HojaTrabajo.Invalidate();
                        }
                        else
                        {
                            pictureBox_HojaTrabajo.Width = panel_HojaTrabajo.Width * (zoomSeleccionado / 2 + 5);
                            pictureBox_HojaTrabajo.Height = panel_HojaTrabajo.Height * (zoomSeleccionado / 2 + 5);
                            pictureBox_HojaTrabajo.Invalidate();
                        }
                    }
                    break;
                case (512):
                    {
                        if (zoomSeleccionado == 1)
                        {
                            pictureBox_HojaTrabajo.Width = panel_HojaTrabajo.Width * 8;
                            pictureBox_HojaTrabajo.Height = panel_HojaTrabajo.Height * 8;
                            pictureBox_HojaTrabajo.Invalidate();
                        }
                        else
                        {
                            pictureBox_HojaTrabajo.Width = panel_HojaTrabajo.Width * (zoomSeleccionado / 2 + 12);
                            pictureBox_HojaTrabajo.Height = panel_HojaTrabajo.Height * (zoomSeleccionado / 2 + 12);
                            pictureBox_HojaTrabajo.Invalidate();
                        }
                    }
                    break;

                default:
                    pictureBox_HojaTrabajo.Width = panel_HojaTrabajo.Width * zoomSeleccionado;
                    pictureBox_HojaTrabajo.Height = panel_HojaTrabajo.Height * zoomSeleccionado;
                    pictureBox_HojaTrabajo.Invalidate();

                    break;
            }
        }

        #endregion


        #region Metodos Auxiliares

        public void Nuevo(int dimensionXX)
        {
            this.dimensionXX = dimensionXX;
            hojaTrab = new HojadeTrabajo(dimensionXX);
            ArreglaHojaTrabajo();
            herrSeleccionada = herramientas.pintar;
            pictureBox_HojaTrabajo.Cursor = new Cursor(Properties.Resources.curPintar.Handle);
            pictureBox_HojaTrabajo.Invalidate();
            pictureBox_VistaPrevia.Invalidate();
            FondoPBX(pictureBox_Pintar);
            label_DimActual.Text = dimensionXX + " x " + dimensionXX + " Píxeles";
            comboBox_Zoom.SelectedIndex = 0;
            Zoom();
        }

        private void CambiarColores(object sender, EventArgs e) //Swap entre las selecciones de colores
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
                (sender as Panel).BackColor = colorDialog.Color;
            if (((Panel)sender).Name == "panel_ColorRejilla")
                checkBox_Rejillas_CheckedChanged(sender, e);
        }

        private void EscogerColores(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    panel_ColorSelec1.BackColor = ((Panel)sender).BackColor;
                    break;

                case MouseButtons.Right:
                    panel_ColorSelec2.BackColor = ((Panel)sender).BackColor;
                    break;
            }
        }

        public void ArreglaHojaTrabajo() //segun las dimension del icono, modifica las dimensiones del pbx hojaTrabajo
        {
            if (dimensionXX < 64)
            {
                pictureBox_HojaTrabajo.Width = panel_HojaTrabajo.Width;
                pictureBox_HojaTrabajo.Height = panel_HojaTrabajo.Height;
            }
            else if (dimensionXX == 64)
            {
                pictureBox_HojaTrabajo.Width = panel_HojaTrabajo.Width * 2;
                pictureBox_HojaTrabajo.Height = panel_HojaTrabajo.Height * 2;
            }
            else if (dimensionXX == 128)
            {
                pictureBox_HojaTrabajo.Width = panel_HojaTrabajo.Width * 4;
                pictureBox_HojaTrabajo.Height = panel_HojaTrabajo.Height * 4;
            }
            else if (dimensionXX == 256)
            {
                pictureBox_HojaTrabajo.Width = panel_HojaTrabajo.Width * 8;
                pictureBox_HojaTrabajo.Height = panel_HojaTrabajo.Height * 8;
            }
            else if (dimensionXX == 512)
            {
                pictureBox_HojaTrabajo.Width = panel_HojaTrabajo.Width * 16;
                pictureBox_HojaTrabajo.Height = panel_HojaTrabajo.Height * 16;
            }
        }

        private void FondoPBX(PictureBox sender) //modifica el background de los picturebox herramientas
        {
            pictureBox_Seleccionar.BackColor = Color.Empty;
            pictureBox_Pegar.BackColor = Color.Empty;
            pictureBox_Copiar.BackColor = Color.Empty;
            pictureBox_Cortar.BackColor = Color.Empty;
            pictureBox_Pintar.BackColor = Color.Empty;
            pictureBox_Rellenar.BackColor = Color.Empty;
            pictureBox_Borrar.BackColor = Color.Empty;
            pictureBox_SeleccionarColor.BackColor = Color.Empty;
            sender.BackColor = Color.Purple;
        }

        #endregion

        
       


        

        

       



        

        
    }
}
