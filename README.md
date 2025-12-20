# Spor Salonu Yönetim Sistemi (Gym Management System)

Bu proje, modern bir spor salonunun yönetim ihtiyaçlarını karşılamak amacıyla geliştirilmiş kapsamlı bir web uygulamasıdır. ASP.NET Core 8.0 MVC mimarisi üzerine inşa edilmiş olup, yapay zeka destekli özellikler ve RESTful API servisleri ile zenginleştirilmiştir.

##  Özellikler

*   **Yönetim Paneli (Admin Dashboard):**
    *   Spor salonu şubeleri, eğitmenler ve hizmetlerin (Yoga, Pilates, Fitness vb.) tam CRUD yönetimi.
    *   Kullanıcı dostu arayüz ve detaylı listeleme.

*   **Randevu Sistemi:**
    *   Üyeler için kolay randevu oluşturma.
    *   Eğitmen ve hizmet bazlı filtreleme.
    *   Çakışma kontrolü (Aynı eğitmenin aynı saatte birden fazla randevusu olamaz).

*   **Yapay Zeka Antrenörü (Gemini AI):**
    *   **Görsel Analiz:** Kullanıcıların yüklediği fotoğrafları analiz ederek vücut tipi ve duruş bozukluklarını tespit eder.
    *   **Kişiselleştirilmiş Program:** Analiz sonucuna göre özel antrenman rutini oluşturur.
    *   **Gelecek Simülasyonu:** Kullanıcının 6 ay sonraki potansiyel formunu görsel olarak üretir (Image Generation).

*   **Raporlama ve API:**
    *   Swagger destekli REST API.
    *   Aylık gelir raporları, popüler eğitmen analizleri ve hizmet kullanım istatistikleri.

##  Kurulum (Installation)

Projeyi yerel ortamınızda çalıştırmak için aşağıdaki adımları izleyin:

1.  **Projeyi Klonlayın:**
    `ash
    git clone https://github.com/kullaniciadi/WebProgramlamaProje.git
    cd WebProgramlamaProje
    `

2.  **Veritabanını Oluşturun:**
    Proje dizininde terminali açın ve veritabanını güncellemek için şu komutu çalıştırın:
    `ash
    dotnet ef database update
    `

3.  **Gemini API Anahtarını Ayarlayın:**
    Yapay zeka özelliklerinin çalışması için Google Gemini API anahtarına ihtiyacınız vardır. Anahtarı güvenli bir şekilde saklamak için .NET User Secrets kullanın:
    `ash
    dotnet user-secrets init
    dotnet user-secrets set "Gemini:ApiKey" "BURAYA_API_ANAHTARINIZI_YAZIN"
    `

4.  **Projeyi Çalıştırın:**
    `ash
    dotnet run
    `

##  Kullanıcı Bilgileri

Sisteme yönetici olarak giriş yapmak ve admin paneline erişmek için aşağıdaki varsayılan bilgileri kullanabilirsiniz:

*   **E-posta:** ogrencinumarasi@sakarya.edu.tr
*   **Şifre:** Sau.123!

##  Teknoloji Yığını

*   **Framework:** .NET 8.0 (ASP.NET Core MVC)
*   **Veritabanı:** Entity Framework Core (SQL Server)
*   **AI Entegrasyonu:** Google Gemini API (REST & JSON)
*   **Frontend:** Bootstrap 5, jQuery Validation
*   **API Dokümantasyonu:** Swagger / OpenAPI

---
*Bu proje Web Programlama dersi kapsamında geliştirilmiştir.*
