System.Random randomNum = new System.Random(); // instância do objeto "random" para gerar coordenadas aleatórias
System.ConsoleKey key; // variavel para capturar input
bool estadoJogo = true; // esta variavel controla se o jogo esta ativo ou não
bool venceuJogo = false; // variavel que seta o resultado final

int d = 8; // linha e coluna para gerar as matrizes
string[,] campo = new string[d, d]; // matriz do jogo
string[,] bombas = new string[d, d]; // matriz das bombas e infos

// posição do cursor do jogador
int x = 0; 
int y = 0;
int slotNum = 0;

const int bandeirasInicio = 8; // ALTERAR AQUI INFLUÊNCIA NO NÚMERO DE BOMBA AUTOMATICAMENTE
int bandeiras = bandeirasInicio;

int CalcularNumeroDoSlot(int cordY, int cordX)
{
    int slotNum = 0;
    int d = bombas.GetLength(0);

    bool IsWithinBounds(int x, int y) => x >= 0 && x < d && y >= 0 && y < d;

    void CheckBomb(int x, int y)
    {
        if (IsWithinBounds(x, y) && bombas[y, x] == "x")
        {
            slotNum++;
        }
    }

    CheckBomb(cordX + 1, cordY); // +1x
    CheckBomb(cordX + 1, cordY + 1);
    CheckBomb(cordX + 1, cordY - 1);

    CheckBomb(cordX - 1, cordY); // -1x
    CheckBomb(cordX - 1, cordY + 1);
    CheckBomb(cordX - 1, cordY - 1);

    CheckBomb(cordX, cordY + 1);
    CheckBomb(cordX, cordY - 1);

    if (cordY == 0 || cordY == d - 1 || cordX == 0 || cordX == d - 1)
    {
        CheckBomb(cordX + 1, cordY);
        CheckBomb(cordX + 1, cordY + 1);
        CheckBomb(cordX + 1, cordY - 1);
        CheckBomb(cordX - 1, cordY);
        CheckBomb(cordX - 1, cordY + 1);
        CheckBomb(cordX - 1, cordY - 1);
        CheckBomb(cordX, cordY + 1);
        CheckBomb(cordX, cordY - 1);
    }

    return slotNum;
}
void CavarBuraco(int cordY, int cordX)
{
	bool continuidade = true;
	int c = 1;
	if (campo[cordY, cordX] == "o") // caso seja um buraco >CONDIÇÃO UNIVERSAL<
	{
		if (bombas[cordY,cordX] == "x") // se cavar a bomba
		{
			estadoJogo = false;
			venceuJogo = false;
		} else if (CalcularNumeroDoSlot(cordY, cordX) > 0) // se tiver uma bomba ao redor
		{
			campo[cordY, cordX] = $"{CalcularNumeroDoSlot(cordY, cordX)}";
		} else
		{
			campo[cordY, cordX] = $" ";

			switch (cordY)
			{
				case 0:
                    campo[cordY + 1, cordX] = $"{CalcularNumeroDoSlot(cordY + 1, cordX)}";
                    break;
                case 7:
                    campo[cordY - 1, cordX] = $"{CalcularNumeroDoSlot(cordY - 1, cordX)}";
                    break;
				default:
                    campo[cordY - 1, cordX] = $"{CalcularNumeroDoSlot(cordY - 1, cordX)}";
                    campo[cordY + 1, cordX] = $"{CalcularNumeroDoSlot(cordY + 1, cordX)}";
                    break;
            }

            switch (cordX)
            {
                case 0:
                    campo[cordY, cordX + 1] = $"{CalcularNumeroDoSlot(cordY, cordX + 1)}";
                    break;
                case 7:
                    campo[cordY, cordX - 1] = $"{CalcularNumeroDoSlot(cordY, cordX - 1)}";
                    break;
                default:
                    campo[cordY, cordX - 1] = $"{CalcularNumeroDoSlot(cordY, cordX - 1)}";
                    campo[cordY, cordX + 1] = $"{CalcularNumeroDoSlot(cordY, cordX + 1)}";
                    break;
            }
        }
	}
}
void InputReagir() // identificar os inputs ao longo do jogo
{
	key = Console.ReadKey().Key;
	switch (key) 
	{
		case ConsoleKey.UpArrow:
			if (y > 0) {
				y--;
			}
			break;
		case ConsoleKey.DownArrow:
			if (y < 7) {
				y++;
			}
			break;
		case ConsoleKey.RightArrow:
			if (x < 7) {
				x++;
			}
			break;
		case ConsoleKey.LeftArrow:
			if (x > 0) {
				x--;
			}
			break;
		case ConsoleKey.Enter:
			if (bandeiras > 0 && campo[y, x] == "o") {
				campo[y, x] = "!";
				bandeiras--;
			} else if (campo[y, x] == "!") {
				bandeiras++;
				campo[y, x] = "o";
			}
			break;
        case ConsoleKey.Spacebar:
			CavarBuraco(y, x);
			break;
        case ConsoleKey.R: // o "R" para o jogo
			estadoJogo = false;
			break;
	}
}
void JogoRefresh() // reseta o jogo
{
	// função que limpa as matrizez
	for (int i = 0; i < d; i++) {
		for (int ii = 0; ii < d; ii++) {
			campo[i, ii] = "o";
			bombas[i, ii] = "";
		}
	}

	int bi = 0;
	int bii = 0;

	// inserção aleatória das bombas de acordo com o número de bandeiras setadas
	for (int ii = 0; ii < bandeiras; ii++) {
		bi = randomNum.Next(0,7);
		bii = randomNum.Next(0,7);
		
		while (bombas[bi,bii] == "x") { // caso a coordenada gerada de pobrema
			bi = randomNum.Next(0,7);
			bii = randomNum.Next(0,7);
		}
		bombas[bi,bii] = "x";
	}

	// analise das celulas para designar o numero
	for (int i = 0; i < d; i++) {
		for (int ii = 0; ii < d; ii++) {
			if (bombas[i, ii] == "") {
				bombas[i, ii] = $"{CalcularNumeroDoSlot(i, ii)}";
			}
		}
	}
}
void DesenharCampo() // desenha o campo no console
{
	int c = 0;
	Console.WriteLine("Colocar/Tirar bandeira: {0,14}\nCavar o buraco: {1,22}", "[Enter]", "[Spacebar]");
	Console.WriteLine("Bandeiras Restantes: {0, 17}", "0" + bandeiras);
	Console.WriteLine($"\n\nX:{x} Y:{y}");

	for (int i = 0; i < d; i++) {
		for (int ii = 0; ii < d; ii++) {
			if (campo[i, ii] == "0")
			{
				campo[i, ii] = " ";
            }
			if (i == y && ii ==x) {
				Console.Write($"#{campo[i, ii]}#");
			} else {
				Console.Write($"[{campo[i, ii]}]");
			}
			//CavarBuraco(i, ii);
			c++;
		}
		Console.WriteLine();
	}
}
void VerificarEstado() // este metodo oira verificar por perdas ou vitórias
{
	int bandeirasEmBomba = 0;
	for (int i = 0; i < d; i++) {
		for (int ii = 0; ii < d; ii++) {
			if (bombas[i, ii] == "x" && campo[i, ii] == "!") {
				bandeirasEmBomba++;
			}
		}
		if (bandeirasEmBomba == bandeirasInicio) {
			venceuJogo = true;
			estadoJogo = false;
		}
	}
}

void JogoMain() // metodo que roda o jogo
{
	JogoRefresh();
	while (estadoJogo == true) {

	Console.Clear();
	DesenharCampo();
	VerificarEstado();
	InputReagir();

    }

	Console.Clear();

	if (venceuJogo == true) {
		Console.WriteLine("Voce ganhou! localizou corretamente todas as bombas");
	} else {
		Console.WriteLine("Voce perdeu!");
	}

	Console.WriteLine("\nDeseja jogar novamente?\n\n[S] Sim\n[N] Nao\n\n");

	#pragma warning disable CS8602 // desabilitar o warning do compiler pq ele é beta
	if (Console.ReadLine().Trim().Equals("S", StringComparison.CurrentCultureIgnoreCase)) {
		estadoJogo = true;
		venceuJogo = false;
		bandeiras = bandeirasInicio;
		JogoRefresh();
		JogoMain();
	}
	#pragma warning restore CS8602 // desabilitar o warning do compiler pq ele é beta

}

JogoMain();
