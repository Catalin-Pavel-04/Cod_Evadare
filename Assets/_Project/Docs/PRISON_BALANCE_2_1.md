# Prototype 2.1 Prison Balance Notes

## Purpose

Prototype 2.1 keeps the existing Prison Escape level flow while making the level clearer, fairer, and more distinct from the Laboratory demo.

## Balancing Goals

- Teach ranged guards in the first room without instant damage.
- Make the locked gate/keycard loop readable.
- Give security lasers inactive, warning, and active states.
- Make the prison yard medium-hard through cover, ranged pressure, and avoidable lasers.
- Make Riot Brute slower but dangerous.
- Make The Warden the hardest Level 2 fight, with cover and side lasers shaping movement.

## Enemy Tuning

| Enemy | HP | Move Speed | Damage | Notes |
|---|---:|---:|---:|---|
| Prison Guard | 3 | 1.55 | 1 contact | Basic melee pressure. |
| Ranged Guard | 3 | 1.15 | 1 projectile | Holds range, telegraphs for 0.25s before shooting. |
| Riot Brute | 20 | 1.15 | 2 contact | Slow, punishing miniboss. |
| The Warden | 58 | 0.95 | 1 projectile/contact | Uses aimed and radial shots; phase 2 below 50% HP. |

## Laser Tuning

| State | Duration | Damage |
|---|---:|---:|
| Inactive | 2.1s | 0 |
| Warning | 0.45s | 0 |
| Active | 1.65s | 1, cooldown 0.9s |

Lasers are positioned to create movement pressure while leaving safe routes around cover.

## Economy Tuning

| Item | Value |
|---|---:|
| Starting money | 45 |
| Health shop price | 20 |
| Ammo shop price | 15 |
| Weapon shop price | 70 |
| Money pickup | 20 |
| Ammo pickup | 18 |
| Health pickup | 2 |

The player can buy one or two small supplies before the harder rooms, but cannot buy everything immediately.

## Target Difficulty

- First guard room: easy-medium.
- Prison yard: medium-hard but fair.
- Riot Brute: pressure fight with room to kite.
- Warden: hardest Level 2 fight, expected to require cover and reload discipline.

Target clear time for a new player is roughly 4-8 minutes.

## Manual Test Checklist

- Main menu loads `Prototype_PrisonLevel_Balanced`.
- Keycard UI starts at 0.
- First ranged guard telegraphs before firing.
- First room clear spawns keycard, ammo, and money.
- Gate blocks movement before keycard and opens with E after keycard.
- Yard lasers cycle inactive -> warning -> active.
- Lasers are avoidable and do not deal instant damage from inactive.
- Riot Brute opens buff choice on death.
- Warden boss health UI appears and phase 2 is visible.
- Victory text says `PRISON ESCAPED`.
- Pause, game over, reload, and restart still behave correctly.

## Known Limitations

- Art and audio are generated placeholders.
- Enemy AI remains prototype-simple.
- Balance has compile validation only; full Play Mode tuning is still required.
