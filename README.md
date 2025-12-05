# Spor Salonu Yönetim ve Randevu Sistemi

Bu proje, bir üniversite ödevi kapsamında geliştirilen **ASP.NET Core MVC** tabanlı bir spor salonu yönetim ve randevu sistemidir. Kullanıcıların spor salonundaki hizmetleri görüntüleyebilmesi, antrenörleri inceleyebilmesi ve randevu alabilmesi hedeflenmektedir.

## 🚀 Teknolojiler

- **Framework:** .NET 8.0 (ASP.NET Core MVC)
- **ORM:** Entity Framework Core
- **Kimlik Doğrulama:** Microsoft Identity
- **Veritabanı:** MSSQL (Planlanan)

## 📂 Proje Yapısı ve Modeller

Proje şu an başlangıç aşamasındadır ve temel **Domain Modelleri (Entities)** oluşturulmuştur:

- **Gym (Spor Salonu):** Salonun adı, adresi ve iletişim bilgilerini tutar.
- **Service (Hizmet):** Fitness, Yoga, Pilates gibi hizmetlerin tanımı, süresi ve ücreti.
- **Trainer (Antrenör):** Antrenörlerin uzmanlık alanları ve bilgileri.
- **Appointment (Randevu):** Üyelerin aldığı randevuların tarihi, durumu ve ücreti.
- **AppUser (Kullanıcı):** Sisteme kayıtlı üyeler (Ad, Soyad vb. ek bilgilerle).

## 🛠️ Kurulum ve Çalıştırma

1.  Projeyi bilgisayarınıza indirin.
2.  Gerekli NuGet paketlerinin yüklendiğinden emin olun (`dotnet restore`).
3.  Veritabanı bağlantı ayarlarını `appsettings.json` dosyasında yapılandırın (İlerleyen aşamalarda eklenecektir).
4.  Projeyi derleyin ve çalıştırın:
    ```bash
    dotnet run
    ```

## 📝 Yapılacaklar (Todo)

- [x] Proje iskeletinin oluşturulması
- [x] Veritabanı modellerinin (Entities) yazılması
- [ ] DbContext sınıfının oluşturulması ve konfigürasyonu
- [ ] Veritabanı Migration işlemlerinin yapılması
- [ ] Controller ve View'lerin (Arayüz) kodlanması
- [ ] Identity entegrasyonu (Kayıt Ol / Giriş Yap)
