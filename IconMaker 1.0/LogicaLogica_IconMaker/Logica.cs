using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Logica_IconMaker
{
    public class HojadeTrabajo
    {
        Color[,] hojaMatriz; //Array principal (Lienzo) en el que se pinta
        Color[,] seleccion;  //Guarda momentaneamente un seleccion de un subarray de hojaMatriz
        Color[,] enMemoria;  //Guarda en memoria un subarray de hojaMatriz
        
        Stack<Color[,]> deshacer; //Pilas para usar las funcionalidades rehacer y deshacer        
        Stack<Color[,]> rehacer;

        Point inicio; //Puntos que indican el inicio y final de las selecciones para poder crear el array enMemoria y seleccion
        Point final;
        
        public HojadeTrabajo(int dimension) //Constructor se inicializan algunas de la variables globales
        {
            hojaMatriz = new Color[dimension, dimension];            
            deshacer = new Stack<Color[,]>();
            rehacer = new Stack<Color[,]>();
            deshacer.Push(ClonarArray(hojaMatriz));  
        }

        public Color this[int i, int j]     //Indexer
        {
            get { return hojaMatriz[i, j]; }
            set { hojaMatriz[i, j] = value; }
        }

        public void Rellena(Point ptoRelleno, Color valRelleno) //Metodo que con un pnto y un color, busca los vecinos adyacentes a el y los cambia de color
        {
            if (hojaMatriz[ptoRelleno.X, ptoRelleno.Y] != valRelleno) 
            {
                Color valInicial = hojaMatriz[ptoRelleno.X, ptoRelleno.Y];
                Queue<Point> vecinos = new Queue<Point>();
                int[] varX = { 0, 1, 0, -1 }; //arrays direcciones
                int[] varY = { -1, 0, 1, 0 };
                vecinos.Enqueue(ptoRelleno);
                hojaMatriz[ptoRelleno.X,ptoRelleno.Y] = valRelleno;                
                while (vecinos.Count > 0) //mientras que la cola no este vacia, coge el primero, lo pinta y encola a los vecinos que cumpluan la condicion
                {
                    Point temp = vecinos.Peek();
                    for (int i = 0; i < varX.Length; i++)
                    {
                        if (EstaEnRango(temp.X + varX[i], temp.Y + varY[i]) && hojaMatriz[temp.X + varX[i], temp.Y + varY[i]] == valInicial)
                        {
                            vecinos.Enqueue(new Point(temp.X + varX[i], temp.Y + varY[i]));
                            hojaMatriz[temp.X + varX[i], temp.Y + varY[i]] = valRelleno;                            
                        }
                    }
                    vecinos.Dequeue();
                }                
            }
        }

        public bool EstaEnRango(int x, int y) //Evalua que dado dos puntos estan dentro del rango del array hojaMatriz
        {
            if (x >= 0 && x < hojaMatriz.GetLength(0) && y >= 0 && y < hojaMatriz.GetLength(1))
                return true;
            return false;
        }

        public void Dibuja(int i, int j, int diametro, Color valor, string patron) //Metodo que filtra, las patrones y llama a cada uno de los metodos adecuados mediante el string patron
        {
            switch (patron)
            {
                case "Borrar": //borrar es un caso especial de cuadrado lo unico que rellena con color Empty
                    DibujaCuadrado(i, j, valor, diametro);
                    break;
                case "Cuadrado":
                    DibujaCuadrado(i, j, valor, diametro);
                    break;
                case "Linea Horizontal":
                    DibujaLineaHorizontal(i, j, valor, diametro);
                    break;
                case "Linea Vertical":
                    DibujaLineaVertical(i, j, valor, diametro);
                    break;
                default:
                    break;
            }
        }   

        public void Seleccion(Point inicio, Point final) //Guarda la seleccion realizada por el ususario
        {
            this.inicio = inicio;
            this.final = final;

            //modifica los puntos para que este en rango
            if (inicio.X < 0)
                inicio.X = 0;
            if (inicio.X >= hojaMatriz.GetLength(0))
                inicio.X = hojaMatriz.GetLength(0) - 1;
            if (inicio.Y < 0)
                inicio.Y = 0;
            if (inicio.Y >= hojaMatriz.GetLength(0))
                inicio.Y = hojaMatriz.GetLength(0) - 1;

            if (final.X < 0)
                final.X = 0;
            if (final.X >= hojaMatriz.GetLength(0))
                final.X = hojaMatriz.GetLength(0) - 1;
            if (final.Y < 0)
                final.Y = 0;
            if (final.Y >= hojaMatriz.GetLength(0))
                final.Y = hojaMatriz.GetLength(0) - 1;

            //se inicializa el array seleccion de las dimensiones correpondientes con la seleccion utilizando los modulos de la resta de las 
            //componntes de los puntos
            seleccion = new Color[Math.Abs(final.X - inicio.X) + 1, Math.Abs(final.Y - inicio.Y) + 1];

            for (int i = Math.Min(inicio.X, final.X), _i = 0; _i < seleccion.GetLength(0); i++, _i++)
            {
                for (int j = Math.Min(inicio.Y, final.Y), _j = 0; _j < seleccion.GetLength(1); j++, _j++)
                {
                    seleccion[_i, _j] = hojaMatriz[i, j];
                }
            }
        }
                
        public void Limpia() //Anula los valores de selecion,inicio y final, para no mantener la seleccion porque ya esta pasa a enMemoria
        {
            seleccion = null;
            inicio = Point.Empty;
            final = Point.Empty;
        }

        #region Dibujar Patrones

        private void DibujaCuadrado(int i, int j, Color valor, int diametro)
        {
            Point inicio = new Point(i - diametro, j - diametro);
            Point final = new Point(i + diametro, j + diametro);
            if (inicio.X < 0)
                inicio.X = 0;
            if (inicio.Y < 0)
                inicio.Y = 0;

            if (final.X >= hojaMatriz.GetLength(0))
                final.X = hojaMatriz.GetLength(0) - 1;
            if (final.Y >= hojaMatriz.GetLength(1))
                final.Y = hojaMatriz.GetLength(1) - 1;

            for (int x = inicio.X; x <= final.X; x++)
            {
                for (int y = inicio.Y; y <= final.Y; y++)
                {
                    hojaMatriz[x, y] = valor;
                }
            }
        }

        private void DibujaLineaHorizontal(int i, int j, Color valor, int diametro)
        {
            Point inicio = new Point(i - diametro, j);
            Point final = new Point(i + diametro, j);
            if (inicio.X < 0)
                inicio.X = 0;

            if (final.X >= hojaMatriz.GetLength(0))
                final.X = hojaMatriz.GetLength(0) - 1;

            for (int x = inicio.X; x <= final.X; x++)
                hojaMatriz[x, inicio.Y] = valor;

        }

        private void DibujaLineaVertical(int i, int j, Color valor, int diametro)
        {
            Point inicio = new Point(i, j - diametro);
            Point final = new Point(i, j + diametro);

            if (inicio.Y < 0)
                inicio.Y = 0;
            if (final.Y >= hojaMatriz.GetLength(1))
                final.Y = hojaMatriz.GetLength(1) - 1;

            for (int y = inicio.Y; y <= final.Y; y++)
                hojaMatriz[inicio.X, y] = valor;

        }

        #endregion

        #region Cortar, Copiar, Pegar

        public void Copiar() //Copia los valores de seleccion a enMemoria
        {
            if (seleccion != null)
            {
                enMemoria = new Color[seleccion.GetLength(0), seleccion.GetLength(1)];
                for (int i = 0; i < seleccion.GetLength(0); i++)
                {
                    for (int j = 0; j < seleccion.GetLength(1); j++)
                    {
                        enMemoria[i, j] = seleccion[i, j];
                    }
                }
            }
        }

        public void Cortar() //LLama a copiar y en los mismo puntos que acciono los pone en Empty
        {
            Copiar();
            if (enMemoria != null)
            {
                for (int i = Math.Min(inicio.X, final.X), _i = 0; _i < seleccion.GetLength(0); i++, _i++)
                {
                    for (int j = Math.Min(inicio.Y, final.Y), _j = 0; _j < seleccion.GetLength(1); j++, _j++)
                    {
                        hojaMatriz[i, j] = Color.Empty;
                    }
                }
            }
        }

        public void Pegar(Point inicio) //A partir de un punto pega lo que tiene guardado enMemoria en hojaMatriz
        {
            if (enMemoria != null)
            {
                this.inicio = inicio;
                final.X = inicio.X + enMemoria.GetLength(0);
                final.Y = inicio.Y + enMemoria.GetLength(1);

                if (final.X >= hojaMatriz.GetLength(0))
                    final.X = hojaMatriz.GetLength(0) - 1;
                if (final.Y >= hojaMatriz.GetLength(0))
                    final.Y = hojaMatriz.GetLength(0) - 1;


                for (int i = Math.Min(inicio.X, final.X), _i = 0; _i < enMemoria.GetLength(0) && i < hojaMatriz.GetLength(0); i++, _i++)
                {
                    for (int j = Math.Min(inicio.Y, final.Y), _j = 0; _j < enMemoria.GetLength(1) && j < hojaMatriz.GetLength(1); j++, _j++)
                    {
                        hojaMatriz[i, j] = enMemoria[_i, _j];
                    }
                }
            }
        }

        #endregion

        #region Deshacer, Rehacer
    
        public void Deshacer()
        {          
            //Saca de la pila un elemento (Color[,]), actualiza hojaTrabajo con dicho elemento, y mete para rehacer 
            if (deshacer.Count > 1)
            {
                if (rehacer.Count > 0 && rehacer.Peek() == deshacer.Peek()) //evalua para que no se te quede la pila vacia y no metas elementos repetidos
                    return;
                rehacer.Push(ClonarArray(deshacer.Pop()));
                Color[,] temp = ClonarArray(deshacer.Peek());
                for (int i = 0; i < temp.GetLength(0); i++)
                {
                    for (int j = 0; j < temp.GetLength(1); j++)
                    {
                        hojaMatriz[i, j] = temp[i, j];
                    }
                }
            }
        }

        public void Rehacer()
        {
            //funcionamiento similiar a deshacer
            if (rehacer.Count > 0)
            {
                if (deshacer.Count > 0 && rehacer.Peek() == deshacer.Peek())
                    return;
                Color[,] temp = ClonarArray(rehacer.Peek());
                for (int i = 0; i < temp.GetLength(0); i++)
                {
                    for (int j = 0; j < temp.GetLength(1); j++)
                    {
                        hojaMatriz[i, j] = temp[i, j];
                    }
                }
                deshacer.Push(rehacer.Pop());
            }   
        }

        #endregion

        public void Modificacion() //Este metodo se llama cuando se realiza un cambio en hojaTrabajo para poder meter los cambios hacia deshacer
        {
            if(deshacer.Peek()!=hojaMatriz)            
                deshacer.Push(ClonarArray(hojaMatriz));

        }

        public void VaciaRehacer()  //Para mantener la misma politica de uso de la herramientas rehacer y deshacer, cuando deshaces y realizas un cambio
            //la pila de rehacer se reinicia
        {
            rehacer = new Stack<Color[,]>();
        }      

        private Color[,] ClonarArray(Color[,] fuente) 
        {
            Color[,] res = new Color[fuente.GetLength(0), fuente.GetLength(1)];
            for (int i = 0; i < fuente.GetLength(0); i++)
            {
                for (int j = 0; j < fuente.GetLength(1); j++)
                {
                    res[i, j] = fuente[i, j];                    
                }                
            }
            return res;
        }

    }

}
