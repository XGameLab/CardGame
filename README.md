```
ゲームプレイ動画  
https://www.youtube.com/watch?v=P6u0MVZYUS0

ソースコードについて
- Assets/Scripts/
  - Title/          # タイトル画面関連
    - DiagonalMove.cs            # 指定された速度と角度でオブジェクトを対角線方向に移動させる
    - GameStateManager.cs        # シーンの状態管理を担当するシングルトンパターンを実装する
    - HoverButton.cs             # ボタンにマウスカーソルを乗せたときにホバー効果として
                                 # 透明度やスケールを変える機能を実装している
    - SoundAd.cs                 # BGMの音量を管理する
    - SpawnFromChildren.cs       # 指定したスプライトを一定間隔で生成する
  - MiniGame/       # ミニゲーム関連
    - CardPressed.cs             # カードがクリックされたときに処理を行う
                                 # プレイヤーのクリックアクションを管理し、2枚のカードが同じタイプか異なるタイプかを比較する
    - ChooseGameMode.cs          # ゲームモードの選択（オフラインまたはオンライン）を管理する
                                 # プレイヤーが選択したモードに応じてゲームを開始する
    - FollowMouse.cs             # ゲームオブジェクトがマウスの動きに従って移動する機能を提供する
    - GameStart.cs               # ゲームの開始時にカードをランダムに配置する機能を提供する
    - ScoreManager.cs            # プレイヤーのスコアを管理し、ゲームの勝者を決定する
                                 # オンラインとオフラインの両方のゲームモードでスコアを同期する
  - StageSelect/    # ステージ画面関連
    - ButtonArrayHandler.cs      # ボタンのクリック、移動、色の変化を処理し、画像とボタンの位置を管理する
    - SceneLoader.cs             # ボタンと対応するシーンの名前を関連付ける
                                 # ボタンがクリックされたときに指定されたシーンをロードする
    - SceneTransition.cs         # キャンバスグループのアルファ値を変化させる
                                 # シーン遷移時のフェードインおよびフェードアウト効果を実現する
  - GalTest/        # 会話システム関連
    - TypewriterEffect.cs        # タイピングエフェクト、背景、音声効果、BGMの切り替えを管理する
                                 # 履歴表示やスキップ機能、ログ機能
                                 # ユーザーがプレイヤーの選択肢を選ぶシーンやゲームの進行に応じて適切なUI要素を表示する
  - Battle/         # バトル関連
    - AudioManager.cs            # 音の再生とボタン操作に関連する
    - BattleAnimationManager.cs  # バトル中のアニメーション管理を担当する
    - BattleInfoManager.cs       # ゲーム中の戦闘情報を管理するもので、プレイヤーと敵のアクションに基づいて情報を更新する
    - ButtonHandler.cs           # 特定のボタンが押されたときのアクションを処理し、関連するイベントをトリガーする
    - CardEffect.cs              # カードの描画、選択、リセットなどの操作を管理する
    - ObjectMover.cs             # 指定されたオブジェクトを特定の位置に移動させる
                                 # 特定の条件に基づいて表示/非表示を切り替えたりするための機能を提供する
    - ObjectShake.cs             # 指定されたUIオブジェクトに震動効果を適用する
    - Player2AI.cs               # ゲーム内でPlayer2の行動を決定するためのAIロジックを提供する
    - PlayerActionHandler.cs     # プレイヤーのアクションを処理し、それに応じたバトル結果を管理する
    - PlayerBalanceAndHP.cs      # ゲームのプレイヤーのHP（ヒットポイント）とバランスの管理を行う
```
