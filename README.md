# Jigsaw Puzzle Case - Zinky Games

Bu proje, Zinky Games Developer Stajyer pozisyonu teknik case çalışması kapsamında geliştirilmiştir.

## 🎮 Özellikler
Proje, istenilen temel özelliklerin yanı sıra genişletilebilir ve modern bir altyapı ile hazırlanmıştır:

* **Dinamik Grid Sistemi:** NxM boyutlarında (Inspector'dan ayarlanabilir) puzzle oluşturma.
* **İki Farklı Kontrol Modu:**
    * *Drag & Drop:* Parçaları sürükleyerek yerleştirme.
    * *Click & Swap:* İki parçaya tıklayarak yerlerini değiştirme.
    * *(Mod seçimi ScriptableObject üzerinden yapılabilir)*.
* **Karıştırma Algoritması:** Parçalar karıştırılırken `Derangement` mantığı kullanılmıştır.
* **Juice / Game Feel:** Parça yerleşimlerinde ve tamamlanma ekranında DOTween ile yumuşak geçişler sağlanmıştır.

## 🛠️ Teknik Detaylar & Mimari

* **New Input System:** Input yönetimi, platform bağımsız ve genişletilebilir olması için yeni sistem ile yazılmıştır.
* **Async/Await (UniTask):** Coroutine yerine, daha performanslı ve okunabilir olması sebebiyle asenkron işlemler UniTask ile yönetilmiştir.
* **DOTween:** Animasyonlar için endüstri standardı olan DOTween kütüphanesi kullanılmıştır.
* **ScriptableObject:** Level konfigürasyonları (Satır, Sütun, Görsel) data odaklı yaklaşım ile ayrılmıştır.

## 🚀 Kurulum ve Çalıştırma

1.  **Unity Versiyonu:** 6000.2.6f2
2.  `Scenes/GameScene` sahnesini açın.
3.  1920x1080 Portrait Önerilir.

## 📦 Kullanılan Kütüphaneler
* [DOTween](http://dotween.demigiant.com/)
* [Cysharp.UniTask](https://github.com/Cysharp/UniTask)
* Unity New Input System
