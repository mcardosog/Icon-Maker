using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace IconMaker_1._0
{
    public partial class NuevoIcono : Form
    {
        public form_IconMaker principalFormulario; //instancia del form principal IconMaker
        public NuevoIcono()
        {
            InitializeComponent();

        }

        private void NuevoIcono_Load(object sender, EventArgs e)
        {            
            comboBox_DimensionHojaTrab.SelectedIndex = 0;
        }

        public void button_Crear_Click(object sender, EventArgs e) //cierra el formulario y llama al metodo nuevo que crea una nueva hojaTrabajo
        {
            this.Close();
            principalFormulario.Nuevo(int.Parse(comboBox_DimensionHojaTrab.Text));
        }
    }
}

