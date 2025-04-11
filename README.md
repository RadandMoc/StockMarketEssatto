# Instrukcja Uruchomienia Aplikacji StockMarketEssatto

Ta instrukcja opisuje, jak skonfigurować środowisko, zbudować i uruchomić aplikację WPF do zarządzania danymi giełdowymi.

## Wymagania Wstępne

1.  **Visual Studio:** Zainstaluj Visual Studio 2022 lub nowsze (wersja Community jest darmowa).
    * Pobierz z: [https://visualstudio.microsoft.com/vs/](https://visualstudio.microsoft.com/vs/)
    * Podczas instalacji upewnij się, że zaznaczyłeś komponent **".NET desktop development"**.
2.  **.NET SDK:** Upewnij się, że masz zainstalowane .NET 8 SDK lub nowsze (zazwyczaj instalowane razem z Visual Studio). Możesz sprawdzić wersję, wpisując `dotnet --version` w wierszu poleceń.
3.  **Klucz API Alpha Vantage:** Aplikacja wymaga klucza API do pobierania danych giełdowych z serwisu Alpha Vantage.
    * Wejdź na stronę: [https://www.alphavantage.co/support/#api-key](https://www.alphavantage.co/support/#api-key)
    * Zarejestruj się (za darmo), aby otrzymać swój unikalny klucz API.
    * **Skopiuj i zapisz ten klucz!** Będzie potrzebny w następnym kroku.

## Konfiguracja Projektu

1.  **Sklonuj lub Pobierz Kod:** Pobierz kod źródłowy projektu z repozytorium Git.
2.  **Otwórz Projekt w Visual Studio:** Otwórz plik rozwiązania (`.sln`) w Visual Studio.
3.  **Przywróć Pakiety NuGet:** Visual Studio powinien automatycznie przywrócić wymagane pakiety NuGet przy pierwszym otwarciu. Jeśli nie, kliknij prawym przyciskiem myszy na rozwiązanie w Solution Explorer i wybierz "Restore NuGet Packages".
4.  **Skonfiguruj Klucz API (User Secrets):**
    * W Solution Explorer, kliknij prawym przyciskiem myszy na projekt `StockMarketEssatto` (lub jakkolwiek nazywa się główny projekt aplikacji).
    * Wybierz opcję **"Manage User Secrets"**. Otworzy się plik `secrets.json`.
    * Wklej do tego pliku poniższą zawartość, zastępując `TWOJ_PRAWDZIWY_KLUCZ_API` **swoim rzeczywistym kluczem API** uzyskanym z Alpha Vantage:
        ```json
        {
          "AlphaVantage": {
            "ApiKey": "TWOJ_PRAWDZIWY_KLUCZ_API"
          }
        }
        ```
    * Zapisz i zamknij plik `secrets.json`. Ten plik jest przechowywany bezpiecznie w Twoim profilu użytkownika i nie jest częścią kodu źródłowego.

## Budowanie i Uruchamianie

1.  **Uruchom Aplikację:**
    * Naciśnij klawisz **F5** lub
    * Kliknij zielony przycisk "Start" (z nazwą projektu) w pasku narzędzi Visual Studio.
2.  **Pierwsze Uruchomienie:**
    * Aplikacja zostanie zbudowana.
    * Przy pierwszym uruchomieniu Entity Framework Core automatycznie utworzy plik bazy danych SQLite (`stockdata.db`) w folderze `%LOCALAPPDATA%` (np. `C:\Users\TWOJA_NAZWA\AppData\Local\stockdata.db`).
    * Pojawi się główne okno aplikacji.

## Używanie Aplikacji

* **Pobieranie Danych:** Wpisz symbol giełdowy (np. `IBM`, `MSFT`, `GOOGL`) w polu "Fetch Quote" i kliknij "Fetch from API". Dane zostaną pobrane i dodane/zaktualizowane w tabeli.
* **Dodawanie:** Kliknij "Add New", wypełnij formularz w nowym oknie i kliknij "Save".
* **Edycja:** Zaznacz wiersz w tabeli i kliknij "Edit Selected". Zmodyfikuj dane w oknie dialogowym i kliknij "Save".
* **Usuwanie:** Zaznacz wiersz i kliknij "Delete Selected". Potwierdź usunięcie.
* **Wyszukiwanie:** Wpisz tekst w polu "Search" i kliknij "Search" (lub naciśnij Enter). Kliknij "Clear", aby wyczyścić filtr.
* **Paginacja:** Użyj przycisków "Previous" i "Next" do nawigacji między stronami, jeśli danych jest więcej niż mieści się na jednej stronie (domyślnie 10).

## Rozwiązywanie Problemów

* **Błąd API Key:** Jeśli widzisz błędy związane z API ("API Key missing", "Invalid API call"), upewnij się, że poprawnie skonfigurowałeś User Secrets (krok 4 w Konfiguracji) i że Twój klucz API z Alpha Vantage jest aktywny i poprawny. Sprawdź też swoje połączenie internetowe. Pamiętaj, że darmowy klucz ma limity użycia.
* **Błędy Bazy Danych:** Jeśli wystąpią błędy przy zapisie (np. "Database error adding quote... Ticker Symbol is unique"), może to oznaczać próbę dodania rekordu z symbolem, który już istnieje. Usuń istniejący rekord lub użyj innego symbolu.
* **Inne Błędy:** Sprawdź komunikaty błędów w oknie Output w Visual Studio, aby uzyskać więcej informacji.