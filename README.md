# Tubes_Terserah
Nama Tim: Terserah  
Strategi Algoritma
Implementasi Algoritma Greedy pada Robocode Tank Royale

Nama Anggota:
1. Angken Muhatun Nufus (124140065)
2. Kevin Savero (124140143)
3. Winda Nainggolan (124140220)


## Deskripsi Proyek

Alternatif 1 (Bot Utama)

Pada bot 1 (Bokir) mengimplementasikan algoritma greedy dengan cara selalu memilih musuh yang paling dekat sebagai target utama untuk diserang. Konsep greedy sendiri merupakan metode pengambilan keputusan yang memilih solusi terbaik pada saat itu juga tanpa mempertimbangkan strategi jangka panjang yang lebih kompleks. Pada program ini, bot akan menyimpan seluruh data musuh yang terdeteksi melalui radar ke dalam dictionary _scannedEnemies, kemudian pada method SelectClosestEnemy() bot melakukan pengecekan jarak setiap musuh menggunakan fungsi DistanceTo(). Musuh dengan jarak paling kecil akan langsung dipilih sebagai target utama. Setelah target terdekat ditemukan, bot akan mengejar musuh tersebut menggunakan method ChaseAndRam() dengan cara memutar badan tank menuju arah musuh, bergerak maju terus menerus, mengarahkan radar dan senjata ke target, lalu menembak ketika arah tembakan sudah cukup akurat. Selain itu, ketika jarak musuh sangat dekat, bot akan masuk ke mode “ram” untuk menabrakkan diri sambil memberikan serangan tambahan. Pendekatan greedy ini membuat bot memiliki perilaku agresif dan responsif karena selalu 
memprioritaskan lawan yang paling dekat dan paling mudah diserang terlebih dahulu.

Alternatif 2

Pada bot 2 (Marlong) mengimplementasikan algoritma greedy dengan cara selalu memilih musuh yang memiliki energi paling rendah sebagai target utama untuk diserang. Konsep greedy yang digunakan pada bot ini adalah mengambil keputusan terbaik pada kondisi saat ini, yaitu menyerang lawan yang paling lemah agar lebih mudah dieliminasi terlebih dahulu. Pada method SelectGreedyTarget(), bot akan memeriksa seluruh musuh yang tersimpan di dalam _scannedEnemies, kemudian membandingkan nilai energi setiap musuh menggunakan atribut Energy. Musuh dengan energi paling kecil akan dipilih sebagai target utama. Setelah target dipilih, bot menjalankan serangan melalui method ExecuteGreedyAttack() dengan mengarahkan senjata ke posisi musuh dan menentukan kekuatan peluru secara adaptif berdasarkan sisa energi lawan. Jika energi lawan sudah sangat kecil maka bot menggunakan peluru kecil agar lebih hemat energi, sedangkan jika energi lawan masih besar maka bot menggunakan peluru dengan daya lebih tinggi untuk memberikan damage yang lebih besar. Selain menyerang, bot juga memiliki strategi bertahan melalui method ExecuteSurvivalMovement() dengan cara bergerak menjauhi kerumunan musuh dan menghindari dinding arena menggunakan method AvoidWalls(). Dengan pendekatan greedy ini, bot menjadi lebih efisien karena memprioritaskan eliminasi musuh yang paling lemah terlebih dahulu sambil tetap menjaga posisi dan keselamatan dirinya di arena.

Alternatif 3

Pada bot 3 (Jarot) mengimplementasikan algoritma greedy dengan cara memilih musuh yang memiliki peluang terkena tembakan paling tinggi sebagai target utama. Konsep greedy yang digunakan adalah mengambil keputusan terbaik pada kondisi saat ini, yaitu memprioritaskan musuh yang bergerak paling lambat karena lebih mudah diprediksi dan ditembak. Pada method SelectHighestHitProbabilityTarget(), bot akan memeriksa seluruh musuh yang tersimpan di dalam _scannedEnemies, lalu membandingkan kecepatan setiap musuh menggunakan atribut Speed. Musuh dengan kecepatan paling rendah akan dipilih sebagai target sniper utama, dan jika terdapat kecepatan yang hampir sama maka bot akan memilih target yang jaraknya lebih dekat agar peluang hit semakin besar. Setelah target dipilih, bot menjalankan strategi predictive shooting pada method ExecuteSniperShot(), yaitu memperkirakan posisi masa depan musuh berdasarkan arah gerak, kecepatan, dan waktu tempuh peluru sehingga tembakan menjadi lebih akurat. Selain itu, kekuatan peluru juga disesuaikan berdasarkan jarak target agar penggunaan energi lebih efisien. Untuk bertahan hidup, bot menerapkan pergerakan strafing atau bergerak menyamping terhadap arah musuh melalui method ExecutePerpendicularMovement() agar lebih sulit terkena serangan lawan, serta menggunakan sistem penghindaran dinding pada method AvoidWallsSniper() supaya tetap berada di area aman arena. Dengan pendekatan greedy ini, bot dapat memaksimalkan peluang mengenai target sambil tetap menjaga mobilitas dan keselamatannya di dalam pertandingan.

Alternatif 4

Pada bot 4 (Garit) mengimplementasikan algoritma greedy dengan cara selalu memilih musuh yang memiliki energi paling rendah sebagai target utama untuk diserang. Konsep greedy yang digunakan adalah mengambil keputusan terbaik pada kondisi saat ini, yaitu memprioritaskan lawan yang paling lemah agar lebih mudah dieliminasi terlebih dahulu. Pada method SelectWeakestTarget(), bot akan memeriksa seluruh musuh yang tersimpan di dalam _detectedEnemies, kemudian membandingkan nilai energi setiap musuh menggunakan atribut Energy. Musuh dengan HP atau energi paling kecil akan dipilih sebagai target utama. Setelah target dipilih, bot akan melakukan serangan menggunakan teknik predictive shooting pada method OnScannedBot(), yaitu memperkirakan posisi masa depan musuh berdasarkan arah gerak, kecepatan, dan waktu tempuh peluru sehingga tembakan menjadi lebih akurat. Selain menyerang, bot juga memiliki sistem movement dan dodge yang cukup adaptif. Bot akan mendeteksi penurunan energi musuh untuk memperkirakan kapan lawan menembak, lalu mengubah arah gerak secara tiba-tiba agar lebih sulit terkena peluru. Pergerakan bot juga dibuat dinamis dengan teknik strafing, menjauh ketika musuh terlalu dekat, serta memiliki sistem penghindaran dinding melalui method OnHitWall(). Dengan pendekatan greedy ini, bot dapat fokus menghabisi lawan yang paling lemah sambil tetap menjaga mobilitas dan kemampuan bertahan hidup di arena pertandingan.




## Requirements dan Instalasi

### Prasyarat Sistem

- **.NET 6.0 SDK** 
- **Robocode Tank Royale** 
- **Jdk 21.0.10** 

### Verifikasi Instalasi .NET

```bash
dotnet --version

### Instalasi NuGet Package

Setiap bot menggunakan package `Robocode.TankRoyale.BotApi`. Package ini otomatis diunduh saat `dotnet restore`. Untuk menginstal manual:

```bash
dotnet add package Robocode.TankRoyale.BotApi --version 0.30.0
```

---

## Cara Menjalankan

1. Buka folder project bot di VS Code. Pastikan file seperti 
Bokir.cs, Jarot.cs, Garit.cs, atau Marlong.cs sudah berada di dalam project C#.

2. Buka terminal di VSCode

3. Masuk ke folder project

4. Compile program dengan (dotnet build)

5. Jalankan dengan (dotnet run)

6. Jika sudah dari langkan 1 sampai 5, buka gui (robocode-tankroyale-gui-0.30.0) dan masukkan bot yang ingin di mainkan

