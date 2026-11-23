# Ball Merge — Unity 2D Physics Puzzle

![Unity](https://img.shields.io/badge/Unity-6000.4.2f1-000000?logo=unity&logoColor=white)
![Language](https://img.shields.io/badge/C%23-239120?logo=csharp&logoColor=white)
![Platform](https://img.shields.io/badge/Platform-Windows-0078D6?logo=windows&logoColor=white)

2학년 1학기 **게임엔진** 과목 기말 팀 프로젝트입니다.
같은 종류의 스포츠 공을 합쳐 더 큰 공으로 성장시키는 2D 물리 기반 병합 퍼즐 게임입니다.

## Overview

플레이어는 상단 발사 위치에서 공을 떨어뜨립니다. 같은 단계의 공이 맞닿으면 다음 단계의 공으로 합쳐지고 점수를 얻습니다.
공이 위험선 위까지 쌓이면 게임이 종료되며, 최종 점수는 로컬 Top 5 순위표에 기록됩니다.

**합체 단계 (8단계)**

| Lv | 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 |
|----|---|---|---|---|---|---|---|---|
| 공 | 탁구공 | 테니스공 | 야구공 | 볼링공 | 핸드볼공 | 배구공 | 축구공 | 농구공 |

## Features

- **물리 기반 병합** — `Rigidbody2D` / `CircleCollider2D` 충돌로 같은 레벨 공을 합체
- **점수 기반 난이도** — 점수가 100 / 300 / 600 / 1000점을 넘을 때마다 스폰 가능한 최대 레벨이 증가
- **Next 미리보기와 낙하 가이드** — 다음 공과 낙하 위치를 미리 표시
- **오브젝트 풀링** — 공과 이펙트를 재사용해 런타임 인스턴스 생성·GC 부담을 줄임
- **Top 5 순위표** — `PlayerPrefs`에 저장, 신기록 강조 표시
- **게임 오버 연출 및 UI** — 결과 화면, 재시작 버튼
- **세로형(9:16) 화면 구성** — 카메라 전체 화면 표시, 좌우 사이드바는 Canvas 패널로 처리

## Controls

| 입력 | 동작 |
|------|------|
| 마우스 이동 | 발사 위치 조절 |
| 마우스 왼쪽 버튼 | 공 떨어뜨리기 |

## Getting Started

### Play (Windows build)

저장소 루트의 `team12.exe`를 실행합니다.

### Open in Unity

1. Unity Hub에서 **Unity 6000.4.2f1**을 설치합니다.
2. Unity Hub → **Add** → 이 저장소 폴더를 선택합니다.
3. `Assets/Scenes/GAME.unity`를 열고 **Play**를 누릅니다.

## Project Structure

```
.
├── Assets
│   ├── C#            # 게임 스크립트
│   ├── Scenes        # GAME.unity (메인 씬, 빌드 설정에 등록됨)
│   ├── Prefabs       # Dongle (공), Effect (합체 이펙트)
│   ├── Animation     # Level 0–7 애니메이션, AcDongle 컨트롤러
│   ├── image         # 공 및 UI 스프라이트
│   ├── BGM           # 배경음악
│   └── TextMesh Pro  # TMP 리소스
├── Packages          # Unity 패키지 매니페스트
├── ProjectSettings   # Unity 프로젝트 설정
└── team12.exe 외     # Windows 빌드 결과물
```

## Architecture

| 스크립트 | 책임 |
|----------|------|
| `GameManager` | 공 생성, 오브젝트 풀, 입력 처리, 점수별 스폰 레벨, 게임 오버 판정 |
| `Dongle` | 공 단위 동작: 낙하, 동일 레벨 합체, 위험선 판정 |
| `NextPreview`, `DropGuide` | 다음 공 미리보기, 낙하 위치 가이드 |
| `Score`, `ScoreManager` | 현재 점수 계산, Top 5 저장·불러오기 (`PlayerPrefs`) |
| `RankingBoard`, `ScoreBoardUI` | 순위표 표시 |
| `GameOverEffect`, `GameOverUI` | 게임 오버 연출과 결과 화면 |
| `SidebarUI`, `ResolutionManager` | 사이드바 UI, 카메라 화면 설정 |

## Tech Stack

- Unity 6000.4.2f1 (2D Physics)
- C#
- TextMesh Pro
- Unity Animator / Animation

## Known Limitations

- `Assets/image/11.짐볼 최종.png`는 현재 게임에 연결되지 않은 미사용 에셋입니다. 9단계(짐볼)를 추가하려면 `Level 8` 애니메이션과 `GameManager`의 레벨 설정 확장이 필요합니다.
- 순위표는 기기 로컬(`PlayerPrefs`)에만 저장되며 온라인 랭킹은 지원하지 않습니다.
