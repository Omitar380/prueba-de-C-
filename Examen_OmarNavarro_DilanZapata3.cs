using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examen_DE_OmarNavarro_DIlanZapata
{
    public class Pasajero
    {
        private string nombre;
        private int edad;
        private string documento;

        public string Nombre
        {
            get => nombre;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    nombre = value.Trim();
            }
        }
        public int Edad
        {
            get => edad;
            set
            {
                if (value >= 0 && value <= 120)
                    edad = value;
            }
        }
        public string Documento
        {
            get => documento;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    documento = value.Trim();
            }
        }

        public Pasajero(string nombre, int edad, string documento)
        {
            Nombre = nombre;
            Edad = edad;
            Documento = documento;
        }

        public override string ToString()
        {
            return $"Nombre: {Nombre}, Edad: {Edad}, Documento: {Documento}";
        }
    }


    public class Recorrido
    {
        private string origen;
        private string destino;
        private double distanciaKm;
        private DateTime fecha;

        public string Origen
        {
            get => origen;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    origen = value.Trim();
            }
        }
        public string Destino
        {
            get => destino;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    destino = value.Trim();
            }
        }
        public double DistanciaKm
        {
            get => distanciaKm;
            set
            {
                if (value >= 0)
                    distanciaKm = value;
            }
        }
        public DateTime Fecha
        {
            get => fecha;
            set => fecha = value;
        }

        public Recorrido(string origen, string destino, double distanciaKm, DateTime fecha)
        {
            Origen = origen;
            Destino = destino;
            DistanciaKm = distanciaKm;
            Fecha = fecha;
        }

        public override string ToString()
        {
            return $"Origen: {Origen}, Destino: {Destino}, Distancia: {DistanciaKm} km, Fecha: {Fecha:dd/MM/yyyy}";
        }
    }


    public abstract class Servicio
    {
        private string codigo;
        private List<Pasajero> pasajeros;
        private List<Recorrido> recorridos;

        public string Codigo
        {
            get => codigo;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    codigo = value.Trim();
            }
        }
        public IReadOnlyList<Pasajero> Pasajeros => pasajeros.AsReadOnly();
        public IReadOnlyList<Recorrido> Recorridos => recorridos.AsReadOnly();

        protected Servicio(string codigo)
        {
            Codigo = codigo;
            pasajeros = new List<Pasajero>();
            recorridos = new List<Recorrido>();
        }

        public bool AgregarPasajero(Pasajero p)
        {
            if (p != null && !pasajeros.Contains(p))
            {
                pasajeros.Add(p);
                return true;
            }
            return false;
        }

        public bool AgregarRecorrido(Recorrido r)
        {
            if (r != null)
            {
                recorridos.Add(r);
                return true;
            }
            return false;
        }

        public abstract string TipoServicio();


        public virtual string ObtenerResumen()
        {
            return $"Servicio {TipoServicio()} - Código: {Codigo}\nPasajeros: {pasajeros.Count}\nRecorridos: {recorridos.Count}";
        }


        public abstract string ProcesarInformacion();
    }


    public class ServicioBasico : Servicio
    {
        public ServicioBasico(string codigo) : base(codigo) { }

        public override string TipoServicio() => "Básico";

        public override string ProcesarInformacion()
        {

            double totalKm = 0;
            foreach (var r in Recorridos)
                totalKm += r.DistanciaKm;

            return $"Distancia total recorrida en servicio básico: {totalKm} km";
        }
    }


    public class ServicioEjecutivo : Servicio
    {
        private int numeroAsientosPremium;

        public int NumeroAsientosPremium
        {
            get => numeroAsientosPremium;
            set
            {
                if (value >= 0)
                    numeroAsientosPremium = value;
            }
        }

        public ServicioEjecutivo(string codigo, int numeroAsientosPremium) : base(codigo)
        {
            NumeroAsientosPremium = numeroAsientosPremium;
        }

        public override string TipoServicio() => "Ejecutivo";

        public override string ObtenerResumen()
        {
            return base.ObtenerResumen() + $"\nAsientos Premium: {NumeroAsientosPremium}";
        }

        public override string ProcesarInformacion()
        {

            if (Pasajeros.Count == 0)
                return "No hay pasajeros para calcular promedio de edad.";

            double sumaEdad = 0;
            foreach (var p in Pasajeros)
                sumaEdad += p.Edad;

            double promedio = sumaEdad / Pasajeros.Count;
            return $"Promedio de edad de pasajeros en servicio ejecutivo: {promedio:F2} años";
        }
    }

    public class ServicioEspecial : Servicio
    {
        private string descripcionPersonalizada;

        public string DescripcionPersonalizada
        {
            get => descripcionPersonalizada;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    descripcionPersonalizada = value.Trim();
            }
        }

        public ServicioEspecial(string codigo, string descripcionPersonalizada) : base(codigo)
        {
            DescripcionPersonalizada = descripcionPersonalizada;
        }

        public override string TipoServicio() => "Especial";

        public override string ObtenerResumen()
        {
            return base.ObtenerResumen() + $"\nDescripción: {DescripcionPersonalizada}";
        }

        public override string ProcesarInformacion()
        {

            if (Recorridos.Count == 0)
                return "No hay recorridos para procesar.";

            DateTime ultimo = DateTime.MinValue;
            foreach (var r in Recorridos)
                if (r.Fecha > ultimo)
                    ultimo = r.Fecha;

            int countMes = 0;
            foreach (var r in Recorridos)
                if (r.Fecha.Year == ultimo.Year && r.Fecha.Month == ultimo.Month)
                    countMes++;

            return $"Recorridos realizados en {ultimo:MMMM yyyy}: {countMes}";
        }
    }

    class Program
    {
        static List<Servicio> servicios = new List<Servicio>();
        static readonly List<string> CiudadesBase = new List<string>
        {
            "Bogotá",
            "Medellín",
            "Cali",
            "Barranquilla",
            "Cartagena"
        };

        static readonly Dictionary<(string origen, string destino), double> DistanciasEntreCiudades = new Dictionary<(string origen, string destino), double>
        {
            [("Bogotá", "Medellín")] = 415,
            [("Bogotá", "Cali")] = 460,
            [("Bogotá", "Barranquilla")] = 780,
            [("Bogotá", "Cartagena")] = 1050,
            [("Medellín", "Cali")] = 640,
            [("Medellín", "Barranquilla")] = 700,
            [("Medellín", "Cartagena")] = 760,
            [("Cali", "Barranquilla")] = 1080,
            [("Cali", "Cartagena")] = 1100,
            [("Barranquilla", "Cartagena")] = 280
        };

        static void Main()
        {
            Console.Title = "Sistema Transporte Intermunicipal";
            bool salir = false;
            while (!salir)
            {
                MostrarMenu();
                string opcion = Console.ReadLine()?.Trim();
                Console.WriteLine();
                switch (opcion)
                {
                    case "1":
                        CrearServicio();
                        break;
                    case "2":
                        AgregarPasajeroAServicio();
                        break;
                    case "3":
                        AgregarRecorridoAServicio();
                        break;
                    case "4":
                        MostrarServicios();
                        break;
                    case "5":
                        EditarInformacionServicio();
                        break;
                    case "6":
                        ProcesarInformacionServicios();
                        break;
                    case "7":
                        LimpiarPantalla();
                        break;
                    case "8":
                        salir = true;
                        Console.WriteLine("Gracias por usar el sistema. ¡Hasta luego!");
                        break;
                    default:
                        Console.WriteLine("Opción inválida. Por favor, intente de nuevo.\n");
                        break;
                }
            }
        }

        static void MostrarMenu()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("===============================================");
            Console.WriteLine("       SISTEMA TRANSPORTE INTERMUNICIPAL       ");
            Console.WriteLine("===============================================");
            Console.ResetColor();
            Console.WriteLine(" 1. Crear nuevo servicio");
            Console.WriteLine(" 2. Agregar pasajero a un servicio");
            Console.WriteLine(" 3. Agregar recorrido a un servicio");
            Console.WriteLine(" 4. Mostrar información de servicios");
            Console.WriteLine(" 5. Editar información de servicio");
            Console.WriteLine(" 6. Procesar información de servicios");
            Console.WriteLine(" 7. Limpiar pantalla");
            Console.WriteLine(" 8. Salir");
            Console.Write("\nSeleccione una opción: ");
        }

        static void CrearServicio()
        {
            Console.WriteLine("Seleccione tipo de servicio a crear:");
            Console.WriteLine(" 0. Cancelar");
            Console.WriteLine(" 1. Básico");
            Console.WriteLine(" 2. Ejecutivo");
            Console.WriteLine(" 3. Especial");
            int tipo = LeerEntero("Opción: ", 0, 3);
            Console.WriteLine();

            if (tipo == 0)
            {
                Console.WriteLine("Creación de servicio cancelada.\n");
                return;
            }

            switch (tipo)
            {
                case 1:
                    string codBasico;
                    do
                    {
                        codBasico = LeerCodigo("Ingrese código para el servicio básico: ");
                        if (ExisteCodigo(codBasico))
                            Console.WriteLine("Código ya existe. Intente otro.\n");
                    }
                    while (ExisteCodigo(codBasico));
                    servicios.Add(new ServicioBasico(codBasico));
                    Console.WriteLine("Servicio básico creado con éxito.\n");
                    break;

                case 2:
                    string codEjecutivo;
                    do
                    {
                        codEjecutivo = LeerCodigo("Ingrese código para el servicio ejecutivo: ");
                        if (ExisteCodigo(codEjecutivo))
                            Console.WriteLine("Código ya existe. Intente otro.\n");
                    }
                    while (ExisteCodigo(codEjecutivo));
                    int asientos = LeerEntero("Ingrese número de asientos premium: ", 0, int.MaxValue);
                    servicios.Add(new ServicioEjecutivo(codEjecutivo, asientos));
                    Console.WriteLine("Servicio ejecutivo creado con éxito.\n");
                    break;

                case 3:
                    string codEspecial;
                    do
                    {
                        codEspecial = LeerCodigo("Ingrese código para el servicio especial: ");
                        if (ExisteCodigo(codEspecial))
                            Console.WriteLine("Código ya existe. Intente otro.\n");
                    }
                    while (ExisteCodigo(codEspecial));
                    string desc = LeerDescripcion("Ingrese descripción personalizada: ");
                    servicios.Add(new ServicioEspecial(codEspecial, desc));
                    Console.WriteLine("Servicio especial creado con éxito.\n");
                    break;
            }
        }

        static bool ExisteCodigo(string codigo)
        {
            foreach (var s in servicios)
                if (s.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        static Servicio BuscarServicioPorCodigo(string codigo)
        {
            foreach (var s in servicios)
                if (s.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase))
                    return s;
            return null;
        }

        static void AgregarPasajeroAServicio()
        {
            if (servicios.Count == 0)
            {
                Console.WriteLine("No hay servicios creados.\n");
                return;
            }

            string codigo = LeerCodigoOCancelar("Ingrese código del servicio para agregar pasajero (0 para cancelar): ");
            if (codigo == null)
            {
                Console.WriteLine("Operación cancelada.\n");
                return;
            }

            var servicio = BuscarServicioPorCodigo(codigo);
            if (servicio == null)
            {
                Console.WriteLine("Servicio no encontrado.\n");
                return;
            }

            string nombre = LeerTextoLetras("Nombre del pasajero: ");
            int edad = LeerEntero("Edad del pasajero: ", 0, 120);
            string doc = LeerTextoAlfanumerico("Documento del pasajero: ");

            var pasajero = new Pasajero(nombre, edad, doc);
            if (servicio.AgregarPasajero(pasajero))
                Console.WriteLine("Pasajero agregado con éxito.\n");
            else
                Console.WriteLine("No se pudo agregar pasajero (posiblemente ya existe).\n");
        }

        static void AgregarRecorridoAServicio()
        {
            if (servicios.Count == 0)
            {
                Console.WriteLine("No hay servicios creados.\n");
                return;
            }

            string codigo = LeerCodigoOCancelar("Ingrese código del servicio para agregar recorrido (0 para cancelar): ");
            if (codigo == null)
            {
                Console.WriteLine("Operación cancelada.\n");
                return;
            }

            var servicio = BuscarServicioPorCodigo(codigo);
            if (servicio == null)
            {
                Console.WriteLine("Servicio no encontrado.\n");
                return;
            }

            string origen = SeleccionarCiudad("Seleccione ciudad de origen:");
            if (origen == null)
            {
                Console.WriteLine("Operación cancelada.\n");
                return;
            }

            string destino;
            do
            {
                destino = SeleccionarCiudad("Seleccione ciudad de destino:");
                if (destino == null)
                {
                    Console.WriteLine("Operación cancelada.\n");
                    return;
                }
                if (destino == origen)
                    Console.WriteLine("El destino no puede ser igual al origen. Elija otra ciudad.\n");
            }
            while (destino == origen);

            double distancia = GetDistancia(origen, destino);
            Console.WriteLine($"Distancia calculada: {distancia} km\n");
            DateTime fecha = LeerFecha("Fecha del recorrido (dd/mm/yyyy): ");

            var recorrido = new Recorrido(origen, destino, distancia, fecha);
            if (servicio.AgregarRecorrido(recorrido))
                Console.WriteLine("Recorrido agregado con éxito.\n");
            else
                Console.WriteLine("No se pudo agregar recorrido.\n");
        }

        static void MostrarServicios()
        {
            if (servicios.Count == 0)
            {
                Console.WriteLine("No hay servicios creados.\n");
                return;
            }

            foreach (var s in servicios)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("-------------------------------------------------");
                Console.ResetColor();
                Console.WriteLine(s.ObtenerResumen());
                Console.WriteLine("\nPasajeros:");
                if (s.Pasajeros.Count == 0)
                    Console.WriteLine("  (ninguno)");
                else
                    foreach (var p in s.Pasajeros)
                        Console.WriteLine("  - " + p);

                Console.WriteLine("\nRecorridos:");
                if (s.Recorridos.Count == 0)
                    Console.WriteLine("  (ninguno)");
                else
                    foreach (var r in s.Recorridos)
                        Console.WriteLine("  - " + r);
                Console.WriteLine();
            }
        }

        static void ProcesarInformacionServicios()
        {
            if (servicios.Count == 0)
            {
                Console.WriteLine("No hay servicios creados.\n");
                return;
            }

            foreach (var s in servicios)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Procesando información para servicio {s.Codigo} ({s.TipoServicio()}):");
                Console.ResetColor();
                Console.WriteLine(s.ProcesarInformacion());
                Console.WriteLine();
            }
        }

        static void LimpiarPantalla()
        {
            Console.Clear();
            Console.WriteLine("Pantalla limpiada. Los datos guardados en el programa permanecen intactos.\n");
        }

        static void EditarInformacionServicio()
        {
            if (servicios.Count == 0)
            {
                Console.WriteLine("No hay servicios creados.\n");
                return;
            }

            string codigo = LeerCodigoOCancelar("Ingrese código del servicio a editar (0 para cancelar): ");
            if (codigo == null)
            {
                Console.WriteLine("Operación cancelada.\n");
                return;
            }

            var servicio = BuscarServicioPorCodigo(codigo);
            if (servicio == null)
            {
                Console.WriteLine("Servicio no encontrado.\n");
                return;
            }

            bool volver = false;
            while (!volver)
            {
                Console.WriteLine("\nCatálogo de edición para servicio {0} ({1}):", servicio.Codigo, servicio.TipoServicio());
                Console.WriteLine(" 1. Cambiar código del servicio");
                Console.WriteLine(" 2. Editar datos específicos del servicio");
                Console.WriteLine(" 3. Editar pasajeros");
                Console.WriteLine(" 4. Editar recorridos");
                Console.WriteLine(" 5. Volver");
                int opcion = LeerEntero("Opción: ", 1, 5);

                switch (opcion)
                {
                    case 1:
                        string nuevoCodigo;
                        do
                        {
                            nuevoCodigo = LeerCodigo("Ingrese nuevo código para el servicio: ");
                            if (!nuevoCodigo.Equals(servicio.Codigo, StringComparison.OrdinalIgnoreCase) && ExisteCodigo(nuevoCodigo))
                                Console.WriteLine("Código ya existe. Intente otro.\n");
                        }
                        while (!nuevoCodigo.Equals(servicio.Codigo, StringComparison.OrdinalIgnoreCase) && ExisteCodigo(nuevoCodigo));
                        servicio.Codigo = nuevoCodigo;
                        Console.WriteLine("Código actualizado.\n");
                        break;

                    case 2:
                        if (servicio is ServicioEjecutivo ejecutivo)
                        {
                            ejecutivo.NumeroAsientosPremium = LeerEntero("Nuevo número de asientos premium: ", 0, int.MaxValue);
                            Console.WriteLine("Asientos premium actualizados.\n");
                        }
                        else if (servicio is ServicioEspecial especial)
                        {
                            especial.DescripcionPersonalizada = LeerDescripcion("Nueva descripción personalizada: ");
                            Console.WriteLine("Descripción actualizada.\n");
                        }
                        else
                        {
                            Console.WriteLine("El servicio básico no tiene datos específicos adicionales para editar.\n");
                        }
                        break;

                    case 3:
                        EditarPasajeros(servicio);
                        break;

                    case 4:
                        EditarRecorridos(servicio);
                        break;

                    case 5:
                        volver = true;
                        break;
                }
            }
        }

        static void EditarPasajeros(Servicio servicio)
        {
            if (servicio.Pasajeros.Count == 0)
            {
                Console.WriteLine("No hay pasajeros en este servicio.\n");
                return;
            }

            for (int i = 0; i < servicio.Pasajeros.Count; i++)
                Console.WriteLine($" {i + 1}. {servicio.Pasajeros[i]}");

            int indice = LeerEntero("Seleccione el pasajero a editar: ", 1, servicio.Pasajeros.Count) - 1;
            var pasajero = servicio.Pasajeros[indice];
            bool volver = false;
            while (!volver)
            {
                Console.WriteLine("\nEditar pasajero:");
                Console.WriteLine(" 1. Nombre");
                Console.WriteLine(" 2. Edad");
                Console.WriteLine(" 3. Documento");
                Console.WriteLine(" 4. Volver");
                int opcion = LeerEntero("Opción: ", 1, 4);

                switch (opcion)
                {
                    case 1:
                        pasajero.Nombre = LeerTextoLetras("Nuevo nombre: ");
                        Console.WriteLine("Nombre actualizado.\n");
                        break;
                    case 2:
                        pasajero.Edad = LeerEntero("Nueva edad: ", 0, 120);
                        Console.WriteLine("Edad actualizada.\n");
                        break;
                    case 3:
                        pasajero.Documento = LeerTextoAlfanumerico("Nuevo documento: ");
                        Console.WriteLine("Documento actualizado.\n");
                        break;
                    case 4:
                        volver = true;
                        break;
                }
            }
        }

        static void EditarRecorridos(Servicio servicio)
        {
            if (servicio.Recorridos.Count == 0)
            {
                Console.WriteLine("No hay recorridos en este servicio.\n");
                return;
            }

            for (int i = 0; i < servicio.Recorridos.Count; i++)
                Console.WriteLine($" {i + 1}. {servicio.Recorridos[i]}");

            int indice = LeerEntero("Seleccione el recorrido a editar: ", 1, servicio.Recorridos.Count) - 1;
            var recorrido = servicio.Recorridos[indice];
            bool volver = false;
            while (!volver)
            {
                Console.WriteLine("\nEditar recorrido:");
                Console.WriteLine(" 1. Origen");
                Console.WriteLine(" 2. Destino");
                Console.WriteLine(" 3. Recalcular distancia");
                Console.WriteLine(" 4. Fecha");
                Console.WriteLine(" 5. Volver");
                int opcion = LeerEntero("Opción: ", 1, 5);

                switch (opcion)
                {
                    case 1:
                        string nuevoOrigen;
                        do
                        {
                            nuevoOrigen = SeleccionarCiudad("Seleccione nuevo origen:");
                            if (nuevoOrigen == recorrido.Destino)
                                Console.WriteLine("El origen no puede ser igual al destino. Elija otra ciudad.\n");
                        }
                        while (nuevoOrigen == recorrido.Destino);
                        recorrido.Origen = nuevoOrigen;
                        recorrido.DistanciaKm = GetDistancia(recorrido.Origen, recorrido.Destino);
                        Console.WriteLine($"Origen actualizado. Distancia recalculada: {recorrido.DistanciaKm} km\n");
                        break;
                    case 2:
                        string nuevoDestino;
                        do
                        {
                            nuevoDestino = SeleccionarCiudad("Seleccione nuevo destino:");
                            if (nuevoDestino == recorrido.Origen)
                                Console.WriteLine("El destino no puede ser igual al origen. Elija otra ciudad.\n");
                        }
                        while (nuevoDestino == recorrido.Origen);
                        recorrido.Destino = nuevoDestino;
                        recorrido.DistanciaKm = GetDistancia(recorrido.Origen, recorrido.Destino);
                        Console.WriteLine($"Destino actualizado. Distancia recalculada: {recorrido.DistanciaKm} km\n");
                        break;
                    case 3:
                        recorrido.DistanciaKm = GetDistancia(recorrido.Origen, recorrido.Destino);
                        Console.WriteLine($"Distancia recalculada: {recorrido.DistanciaKm} km\n");
                        break;
                    case 4:
                        recorrido.Fecha = LeerFecha("Nueva fecha del recorrido (dd/mm/yyyy): ");
                        Console.WriteLine("Fecha actualizada.\n");
                        break;
                    case 5:
                        volver = true;
                        break;
                }
            }
        }

        static string SeleccionarCiudad(string prompt)
        {
            Console.WriteLine(prompt);
            Console.WriteLine(" 0. Cancelar");
            for (int i = 0; i < CiudadesBase.Count; i++)
                Console.WriteLine($" {i + 1}. {CiudadesBase[i]}");
            int opcion = LeerEntero("Ingrese el número de la ciudad: ", 0, CiudadesBase.Count);
            return opcion == 0 ? null : CiudadesBase[opcion - 1];
        }

        static double GetDistancia(string origen, string destino)
        {
            if (origen.Equals(destino, StringComparison.OrdinalIgnoreCase))
                return 0;

            if (DistanciasEntreCiudades.TryGetValue((origen, destino), out double distancia))
                return distancia;

            if (DistanciasEntreCiudades.TryGetValue((destino, origen), out distancia))
                return distancia;

            return 0;
        }

        static string LeerCodigo(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string valor = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(valor))
                {
                    Console.WriteLine("Código inválido. No puede estar vacío.\n");
                    continue;
                }

                bool valido = true;
                foreach (char c in valor)
                    if (!char.IsLetterOrDigit(c))
                    {
                        valido = false;
                        break;
                    }

                if (valido)
                    return valor;

                Console.WriteLine("Código inválido. Use sólo letras y números sin espacios.\n");
            }
        }

        static string LeerCodigoOCancelar(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string valor = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(valor))
                {
                    Console.WriteLine("Código inválido. No puede estar vacío.\n");
                    continue;
                }

                if (valor == "0")
                    return null;

                bool valido = true;
                foreach (char c in valor)
                    if (!char.IsLetterOrDigit(c))
                    {
                        valido = false;
                        break;
                    }

                if (valido)
                    return valor;

                Console.WriteLine("Código inválido. Use sólo letras y números sin espacios.\n");
            }
        }

        static string LeerTextoLetras(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string valor = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(valor))
                {
                    Console.WriteLine("Entrada inválida. No puede estar vacía.\n");
                    continue;
                }

                bool valido = true;
                foreach (char c in valor)
                    if (!char.IsLetter(c) && !char.IsWhiteSpace(c))
                    {
                        valido = false;
                        break;
                    }

                if (valido)
                    return valor;

                Console.WriteLine("Entrada inválida. Sólo se permiten letras y espacios.\n");
            }
        }

        static string LeerTextoAlfanumerico(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string valor = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(valor))
                {
                    Console.WriteLine("Entrada inválida. No puede estar vacía.\n");
                    continue;
                }

                bool valido = true;
                foreach (char c in valor)
                    if (!char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c))
                    {
                        valido = false;
                        break;
                    }

                if (valido)
                    return valor;

                Console.WriteLine("Entrada inválida. Sólo se permiten letras, números y espacios.\n");
            }
        }

        static string LeerDescripcion(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string valor = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(valor))
                {
                    Console.WriteLine("Entrada inválida. No puede estar vacía.\n");
                    continue;
                }

                bool valido = true;
                foreach (char c in valor)
                    if (!char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c) && ".,;:-'\"?!".IndexOf(c) < 0)
                    {
                        valido = false;
                        break;
                    }

                if (valido)
                    return valor;

                Console.WriteLine("Entrada inválida. Sólo se permiten letras, números, espacios y puntuación básica.\n");
            }
        }

        static int LeerEntero(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()?.Trim();
                if (int.TryParse(input, out int valor) && valor >= min && valor <= max)
                    return valor;

                Console.WriteLine($"Entrada inválida. Ingrese un número entre {min} y {max}.\n");
            }
        }

        static double LeerDouble(string prompt, double min)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()?.Trim();
                if (double.TryParse(input, out double valor) && valor >= min)
                    return valor;

                Console.WriteLine($"Entrada inválida. Ingrese un número mayor o igual a {min}.\n");
            }
        }

        static DateTime LeerFecha(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()?.Trim();
                if (DateTime.TryParseExact(input, new[] { "dd/MM/yyyy", "d/M/yyyy" }, null, System.Globalization.DateTimeStyles.None, out DateTime fecha))
                    return fecha;

                Console.WriteLine("Fecha inválida. Use el formato dd/mm/yyyy.\n");
            }
        }
    }
}