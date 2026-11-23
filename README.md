# Team-Project-game-

2학년 1학기 게임엔진 기말 팀 프로젝트. Unity 2D 퍼즐 게임입니다.

## 게임 소개

같은 종류의 공을 합쳐 더 큰 공으로 키우는 병합 퍼즐 게임(수박 게임 방식)입니다.
탁구공 → 테니스공 → 야구공 → 볼링공 → 핸드볼공 → 배구공 → 축구공 → 농구공 (8단계) 순으로 합쳐집니다.
공이 위험선 위로 쌓이면 게임 오버입니다.

- 마우스 이동: 발사 위치 조절
- 마우스 왼쪽 클릭: 공 떨어뜨리기
- 점수가 오를수록 더 큰 공이 나옵니다.
- 다음 공 미리보기, 낙하 위치 가이드, 게임 오버 연출을 제공합니다.
- 최고 점수 Top 5 순위표를 `PlayerPrefs`에 저장합니다.

## 실행 방법

### 빌드 실행
저장소 루트의 `team12.exe`를 실행합니다 (Windows).

### Unity에서 열기
1. Unity Hub에서 Unity **6000.4.2f1** 설치
2. Unity Hub → Add → 이 저장소 폴더 선택
3. `Assets/Scenes/GAME.unity` 열고 Play

## 폴더 구성

| 폴더 | 내용 |
|------|------|
| `Assets/C#` | 게임 스크립트 |
| `Assets/Scenes` | `GAME.unity` (메인 씬) |
| `Assets/Prefabs` | `Dongle`(공), `Effect`(합체 이펙트) |
| `Assets/image` | 공·UI 이미지 |
| `Assets/BGM` | 배경음악 |
| `Assets/Animation` | 애니메이션 |
| `Packages`, `ProjectSettings` | Unity 프로젝트 설정 |
| `team12.exe`, `team12_Data` 등 | Windows 빌드 결과물 |

## 주요 스크립트

| 스크립트 | 역할 |
|----------|------|
| `GameManager` | 공 생성·오브젝트 풀·입력·점수별 스폰 레벨·게임 오버 |
| `Dongle` | 공 하나의 동작: 낙하, 같은 레벨끼리 합체, 위험 판정 |
| `NextPreview` / `DropGuide` | 다음 공 미리보기 / 낙하 위치 가이드 |
| `Score` / `ScoreManager` | 현재 점수 / 최고 점수 Top 5 저장 |
| `RankingBoard` / `ScoreBoardUI` | 순위표 표시 |
| `GameOverEffect` / `GameOverUI` | 게임 오버 연출과 화면 |
| `SidebarUI` / `ResolutionManager` | 사이드바 UI / 해상도 대응 |

## 개발 환경

- Unity 6000.4.2f1
- C#
- TextMesh Pro
