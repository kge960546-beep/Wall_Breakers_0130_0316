## Third-Party Music Credits

### Towball’s Crossing (Music Pack)
- Author: Towball
- Source: https://towball.itch.io/towballs-crossing
- License: Creative Commons Attribution 4.0 International (CC BY 4.0)
  - License Text: https://creativecommons.org/licenses/by/4.0/
- Usage: Background music (BGM) included in this project.
- Changes:
  - None (original files used as provided).
  - If modified: (e.g., trimmed / looped / normalized / converted format)


# 1. 커밋 메시지 기본 구조

type: Subject (제목)

body (본문, 생략 가능)

footer (꼬리말, 생략 가능)

---

# 2. 자주 사용하는 타입(Type) 정의

| 타입 | 의미 | 예시 |
| --- | --- | --- |
| Feat: | 새로운 기능 추가 | Feat: Add 캐릭터 벽 충돌 로직 구현 |
| Fix: | 버그 수정 | Fix: 점프 시 바닥을 뚫고 내려가는 현상 수정 |
| Design: | UI/UX 디자인 변경 | Design: 메인 메뉴 버튼 레이아웃 수정 |
| Asset: | 에셋(모델, 사운드, 이미지) 추가/변경 | Asset: 벽 파편 메쉬 및 텍스처 추가 |
| Refactor: | 코드 리팩토링 (기능 변화 없음) | Refactor: GameManager 싱글톤 구조 개선 |
| Docs: | 문서 수정 (README 등) | Docs: 기능 명세서 기획안 업데이트 |
| Chore: | 빌드 설정, 패키지 매니저 등 단순 작업 | Chore: URP 패키지 업데이트 |

---

# 3. 작성 규칙 (Best Practice)

- **첫 글자는 대문자, 마침표는 생략:** `Feat: Add player controller` (O) / `feat: add player controller.` (X)
- **현재형 동사 사용:** 'Fixed'보다는 'Fix', 'Added'보다는 'Add'를 권장합니다.
- **한글 vs 영어:** 팀원들과 상의해서 정하시되, 한국인 팀 프로젝트라면 직관적인 **한글**을 사용하는 것이 소통이 더 빠를 수 있습니다. (예: `Feat: 벽 파괴 이펙트 구현`)

---

# 4. 실제 적용 예시

Feat: 캐릭터 이동 및 조작 시스템 구현

Asset: 벽 오브젝트 프리팹 및 재질 생성

Fix: 벽 충돌 시 점수가 이중으로 오르는 버그 수정

Design: 결과 화면(Game Over) 팝업 UI 추가
