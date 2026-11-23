# ⚽ Ball Merge

**Unity 2D 물리 기반 병합 퍼즐 게임** · 2학년 1학기 게임엔진 기말 팀 프로젝트

![Unity](https://img.shields.io/badge/Unity-6000.4.2f1-000000?logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?logo=csharp&logoColor=white)
![Render](https://img.shields.io/badge/URP-2D%20Renderer-blue)
![Platform](https://img.shields.io/badge/Platform-Windows-0078D6?logo=windows&logoColor=white)

---

## 📌 목차

1. [게임 소개](#-게임-소개)
2. [플레이 방법](#-플레이-방법)
3. [주요 기능](#-주요-기능)
4. [실행하기](#-실행하기)
5. [프로젝트 구조](#-프로젝트-구조)
6. [스크립트 구조](#-스크립트-구조)
7. [기술 스택](#-기술-스택)
8. [알려진 제한 사항](#-알려진-제한-사항)

---

## 🎮 게임 소개

같은 종류의 스포츠 공을 합쳐 더 큰 공으로 키우는 게임입니다.
상단에서 공을 떨어뜨리고, **같은 단계의 공끼리 닿으면 다음 단계로 합쳐지며 점수**를 얻습니다.
공이 위험 구역에 오래 머무르면 게임 오버이며, 최종 점수는 **Top 5 순위표**에 기록됩니다.

### 합체 단계 (총 8단계)

| 단계 | Lv 0 | Lv 1 | Lv 2 | Lv 3 | Lv 4 | Lv 5 | Lv 6 | Lv 7 |
|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|
| 공 | 탁구공 | 테니스공 | 야구공 | 볼링공 | 핸드볼공 | 배구공 | 축구공 | 농구공 |

---

## 🕹️ 플레이 방법

| 입력 | 동작 |
|------|------|
| 마우스 이동 | 발사 위치 조절 (좌우 벽 안쪽으로 제한) |
| 마우스 왼쪽 클릭 | 공 떨어뜨리기 |

**규칙**

- 같은 단계의 공 두 개가 맞닿으면 한 단계 큰 공으로 합쳐지고, 단계별 점수가 올라갑니다.
- 공이 위험 구역에 **2초 이상** 머물면 붉게 변하고, **5초 이상** 머물면 게임 오버입니다.
- 점수가 **100 / 300 / 600 / 1000점**을 넘으면 등장하는 공의 최대 단계가 1씩 올라갑니다. (시작은 Lv 0~1, 최대 Lv 5)
- 게임 오버 시 카메라 흔들림과 붉은 화면 효과가 나오고, 결과 화면에서 재시작할 수 있습니다.

---

## ✨ 주요 기능

| 기능 | 설명 |
|------|------|
| 물리 기반 병합 | `Rigidbody2D`, `CircleCollider2D` 충돌로 같은 단계 공을 합체 |
| 점수 기반 난이도 | 점수 구간마다 스폰 가능한 최대 단계 상승 |
| 다음 공 미리보기 | 다음에 나올 공을 미리 표시 |
| 낙하 가이드 | 공이 떨어질 위치를 점선으로 표시 |
| 오브젝트 풀링 | 공과 이펙트를 재사용해 인스턴스 생성·GC 부담 감소 |
| Top 5 순위표 | `PlayerPrefs`에 저장, 신기록 강조 표시 |
| 게임 오버 연출 | 카메라 흔들기 + 붉은 플래시 + 결과 UI |
| 세로형 화면 | 9:16 기준 구성, 좌우 사이드바는 Canvas 패널 |
| BGM / 효과음 | 배경음악, 합체 효과음 |

---

## 🚀 실행하기

### 방법 1. 빌드 실행 (Windows)

저장소 루트의 **`team12.exe`** 를 실행하세요.

### 방법 2. Unity에서 열기

1. Unity Hub에서 **Unity 6000.4.2f1** 설치
2. Unity Hub → **Add** → 이 저장소 폴더 선택
3. `Assets/Scenes/GAME.unity` 열기 → **Play**

> 첫 실행 시 `Library` 폴더가 자동 생성되어 시간이 걸릴 수 있습니다.

---

## 📁 프로젝트 구조

```
Team-Project-game-/
├── Assets/
│   ├── C#/               게임 스크립트
│   ├── Scenes/           GAME.unity (메인 씬)
│   ├── Prefabs/          Dongle(공), Effect(합체 이펙트)
│   ├── Animation/        Level 0–7 애니메이션, AcDongle 컨트롤러
│   ├── image/            공·UI 스프라이트
│   ├── BGM/              배경음악
│   ├── Settings/         URP / 2D Renderer 설정
│   └── TextMesh Pro/     TMP 리소스
├── Packages/             Unity 패키지 매니페스트
├── ProjectSettings/      Unity 프로젝트 설정
├── team12.exe            Windows 빌드 실행 파일
└── team12_Data/ 외       빌드 구성 파일
```

---

## 🧩 스크립트 구조

| 분류 | 스크립트 | 역할 |
|------|----------|------|
| 게임 로직 | `GameManager` | 공 생성, 오브젝트 풀, 입력, 점수별 스폰 단계, 게임 오버 처리 |
| 게임 로직 | `Dongle` | 공 한 개의 동작: 낙하, 동일 단계 합체, 위험 구역 판정 |
| 점수 | `Score` | 현재 점수 계산·표시 |
| 점수 | `ScoreManager` | Top 5 점수 저장·불러오기 (`PlayerPrefs`) |
| UI | `NextPreview` | 다음 공 미리보기 |
| UI | `DropGuide` | 낙하 위치 점선 가이드 |
| UI | `RankingBoard`, `ScoreBoardUI` | 순위표 표시 |
| UI | `GameOverUI`, `GameOverEffect` | 결과 화면, 게임 오버 연출 |
| UI | `SidebarUI`, `ResolutionManager` | 좌우 사이드바, 카메라 화면 설정 |

---

## 🛠️ 기술 스택

- **엔진:** Unity 6000.4.2f1
- **언어:** C#
- **렌더링:** Universal Render Pipeline (2D Renderer)
- **UI:** Unity UI, TextMesh Pro
- **물리/애니메이션:** Unity 2D Physics, Animator

---

## ⚠️ 알려진 제한 사항

- `Assets/image/11.짐볼 최종.png`는 게임에 연결되지 않은 **미사용 에셋**입니다. 9단계(짐볼)를 추가하려면 `Level 8` 애니메이션과 `GameManager` 설정 확장이 필요합니다.
- 순위표는 기기 로컬(`PlayerPrefs`)에만 저장되며 온라인 랭킹은 지원하지 않습니다.
