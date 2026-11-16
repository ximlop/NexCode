require("dotenv").config();

const express = require("express");
const cors = require("cors");
const bcrypt = require("bcryptjs");
const jwt = require("jsonwebtoken");
const { MongoClient, ObjectId } = require("mongodb");

const app = express();
const port = process.env.PORT || 3000;

app.use(cors());
app.use(express.json());

let usersCollection;

function createToken(userId) {
    return jwt.sign(
        { sub: userId.toString() },
        process.env.JWT_SECRET,
        { expiresIn: "7d" }
    );
}

function publicUser(user) {
    return {
        id: user._id.toString(),
        displayName: user.displayName,
        email: user.email,
        points: user.points,
        settings: user.settings,
        progress: user.progress,
        mistakes: user.mistakes,
        missions: user.missions,
        achievements: user.achievements
    };
}

function authenticate(req, res, next) {
    const authorization = req.headers.authorization;

    if (!authorization?.startsWith("Bearer ")) {
        return res.status(401).json({
            message: "Sesión requerida."
        });
    }

    const token = authorization.substring(7);

    try {
        const payload = jwt.verify(
            token,
            process.env.JWT_SECRET
        );

        req.userId = payload.sub;
        next();
    }
    catch {
        return res.status(401).json({
            message: "Sesión inválida o vencida."
        });
    }
}

app.get("/api/health", (req, res) => {
    res.json({
        ok: true,
        message: "API NexCode funcionando."
    });
});

app.post("/api/auth/register", async (req, res) => {
    try {
        const displayName =
            String(req.body.displayName || "").trim();

        const email =
            String(req.body.email || "")
                .trim()
                .toLowerCase();

        const password =
            String(req.body.password || "");

        if (!displayName || !email || password.length < 8) {
            return res.status(400).json({
                message:
                    "Ingresa nombre, correo y una contraseña "
                    + "de al menos 8 caracteres."
            });
        }

        const existingUser =
            await usersCollection.findOne({ email });

        if (existingUser) {
            return res.status(409).json({
                message: "El correo ya está registrado."
            });
        }

        const passwordHash =
            await bcrypt.hash(password, 12);

        const newUser = {
            displayName,
            email,
            passwordHash,
            points: 0,

            settings: {
                theme: "light",
                learningPath: "",
                tutorialCompleted: false
            },

            progress: {
                Python: {
                    highestUnlockedLevel: 1,
                    completedLevels: []
                },
                Java: {
                    highestUnlockedLevel: 1,
                    completedLevels: []
                }
            },

            mistakes: {
                Python: {},
                Java: {}
            },

            missions: [],
            achievements: [],

            createdAt: new Date(),
            updatedAt: new Date()
        };

        const result =
            await usersCollection.insertOne(newUser);

        newUser._id = result.insertedId;

        const token = createToken(result.insertedId);

        return res.status(201).json({
            message: "Cuenta creada correctamente.",
            token,
            user: publicUser(newUser)
        });
    }
    catch (error) {
        console.error(error);

        if (error.code === 11000) {
            return res.status(409).json({
                message: "El correo ya está registrado."
            });
        }

        return res.status(500).json({
            message: "No se pudo crear la cuenta."
        });
    }
});

app.post("/api/auth/login", async (req, res) => {
    try {
        const email =
            String(req.body.email || "")
                .trim()
                .toLowerCase();

        const password =
            String(req.body.password || "");

        const user =
            await usersCollection.findOne({ email });

        if (!user) {
            return res.status(401).json({
                message: "Correo o contraseña incorrectos."
            });
        }

        const validPassword =
            await bcrypt.compare(
                password,
                user.passwordHash
            );

        if (!validPassword) {
            return res.status(401).json({
                message: "Correo o contraseña incorrectos."
            });
        }

        const token = createToken(user._id);

        return res.json({
            message: "Sesión iniciada.",
            token,
            user: publicUser(user)
        });
    }
    catch (error) {
        console.error(error);

        return res.status(500).json({
            message: "No se pudo iniciar sesión."
        });
    }
});

app.get("/api/user/me", authenticate, async (req, res) => {
    try {
        const user =
            await usersCollection.findOne({
                _id: new ObjectId(req.userId)
            });

        if (!user) {
            return res.status(404).json({
                message: "Usuario no encontrado."
            });
        }

        return res.json({
            user: publicUser(user)
        });
    }
    catch (error) {
        console.error(error);

        return res.status(500).json({
            message: "No se pudieron cargar los datos."
        });
    }
});

app.patch(
    "/api/user/settings",
    authenticate,
    async (req, res) => {
        try {
            const changes = {};

            if (
                req.body.theme === "light" ||
                req.body.theme === "dark"
            ) {
                changes["settings.theme"] =
                    req.body.theme;
            }

            if (
                req.body.learningPath === "" ||
                req.body.learningPath === "Python" ||
                req.body.learningPath === "Java"
            ) {
                changes["settings.learningPath"] =
                    req.body.learningPath;
            }

            if (
                typeof req.body.tutorialCompleted
                === "boolean"
            ) {
                changes["settings.tutorialCompleted"] =
                    req.body.tutorialCompleted;
            }

            if (Object.keys(changes).length === 0) {
                return res.status(400).json({
                    message:
                        "No se recibieron configuraciones válidas."
                });
            }

            changes.updatedAt = new Date();

            await usersCollection.updateOne(
                { _id: new ObjectId(req.userId) },
                { $set: changes }
            );

            const user =
                await usersCollection.findOne({
                    _id: new ObjectId(req.userId)
                });

            return res.json({
                message: "Configuración actualizada.",
                user: publicUser(user)
            });
        }
        catch (error) {
            console.error(error);

            return res.status(500).json({
                message:
                    "No se pudo actualizar la configuración."
            });
        }
    }
);

app.post(
    "/api/user/progress/complete",
    authenticate,
    async (req, res) => {
        try {
            const learningPath = req.body.learningPath;
            const levelNumber =
                Number(req.body.levelNumber);

            if (
                !["Python", "Java"].includes(learningPath) ||
                !Number.isInteger(levelNumber) ||
                levelNumber < 1 ||
                levelNumber > 6
            ) {
                return res.status(400).json({
                    message: "Progreso inválido."
                });
            }

            const userId = new ObjectId(req.userId);

            const existingUser =
                await usersCollection.findOne({
                    _id: userId
                });

            if (!existingUser) {
                return res.status(404).json({
                    message: "Usuario no encontrado."
                });
            }

            const completedLevels =
                existingUser.progress?.[learningPath]
                    ?.completedLevels || [];

            const firstCompletion =
                !completedLevels.includes(levelNumber);

            const nextLevel =
                Math.min(levelNumber + 1, 6);

            const completedField =
                `progress.${learningPath}.completedLevels`;

            const highestField =
                `progress.${learningPath}.highestUnlockedLevel`;

            const update = {
                $addToSet: {
                    [completedField]: levelNumber
                },
                $max: {
                    [highestField]: nextLevel
                },
                $set: {
                    updatedAt: new Date()
                }
            };

            if (firstCompletion) {
                update.$inc = {
                    points: 10
                };
            }

            await usersCollection.updateOne(
                { _id: userId },
                update
            );

            const updatedUser =
                await usersCollection.findOne({
                    _id: userId
                });

            return res.json({
                message: firstCompletion
                    ? "Nivel completado. Ganaste 10 puntos."
                    : "Nivel repasado correctamente.",

                pointsAwarded:
                    firstCompletion ? 10 : 0,

                user: publicUser(updatedUser)
            });
        }
        catch (error) {
            console.error(error);

            return res.status(500).json({
                message:
                    "No se pudo guardar el progreso."
            });
        }
    }
);

app.post(
    "/api/user/mistakes",
    authenticate,
    async (req, res) => {
        try {
            const learningPath = req.body.learningPath;
            const levelNumber =
                Number(req.body.levelNumber);

            if (
                !["Python", "Java"].includes(learningPath) ||
                !Number.isInteger(levelNumber) ||
                levelNumber < 1 ||
                levelNumber > 6
            ) {
                return res.status(400).json({
                    message: "Error de nivel inválido."
                });
            }

            const mistakeField =
                `mistakes.${learningPath}.${levelNumber}`;

            await usersCollection.updateOne(
                { _id: new ObjectId(req.userId) },
                {
                    $inc: {
                        [mistakeField]: 1
                    },
                    $set: {
                        updatedAt: new Date()
                    }
                }
            );

            return res.json({
                message: "Error registrado."
            });
        }
        catch (error) {
            console.error(error);

            return res.status(500).json({
                message: "No se pudo registrar el error."
            });
        }
    }
);

app.patch(
    "/api/user/password",
    authenticate,
    async (req, res) => {
        try {
            const currentPassword =
                String(req.body.currentPassword || "");

            const newPassword =
                String(req.body.newPassword || "");

            if (!currentPassword || newPassword.length < 8) {
                return res.status(400).json({
                    message:
                        "La nueva contraseña debe tener al menos 8 caracteres."
                });
            }

            if (currentPassword === newPassword) {
                return res.status(400).json({
                    message:
                        "La nueva contraseña debe ser diferente."
                });
            }

            const userId = new ObjectId(req.userId);

            const user = await usersCollection.findOne({
                _id: userId
            });

            if (!user) {
                return res.status(404).json({
                    message: "Usuario no encontrado."
                });
            }

            const validPassword = await bcrypt.compare(
                currentPassword,
                user.passwordHash
            );

            if (!validPassword) {
                return res.status(401).json({
                    message:
                        "La contraseña actual es incorrecta."
                });
            }

            const passwordHash = await bcrypt.hash(
                newPassword,
                12
            );

            await usersCollection.updateOne(
                { _id: userId },
                {
                    $set: {
                        passwordHash,
                        updatedAt: new Date()
                    }
                }
            );

            return res.json({
                message:
                    "Contraseña actualizada correctamente."
            });
        }
        catch (error) {
            console.error(error);

            return res.status(500).json({
                message:
                    "No se pudo cambiar la contraseña."
            });
        }
    }
);

app.post(
    "/api/user/reset-progress",
    authenticate,
    async (req, res) => {
        try {
            const userId =
                new ObjectId(req.userId);

            const result =
                await usersCollection.updateOne(
                    { _id: userId },
                    {
                        $set: {
                            points: 0,

                            "settings.learningPath": "",

                            progress: {
                                Python: {
                                    highestUnlockedLevel: 1,
                                    completedLevels: []
                                },
                                Java: {
                                    highestUnlockedLevel: 1,
                                    completedLevels: []
                                }
                            },

                            mistakes: {
                                Python: {},
                                Java: {}
                            },

                            missions: [],
                            achievements: [],
                            updatedAt: new Date()
                        }
                    }
                );

            if (result.matchedCount === 0) {
                return res.status(404).json({
                    message: "Usuario no encontrado."
                });
            }

            return res.json({
                message:
                    "El progreso fue restablecido."
            });
        }
        catch (error) {
            console.error(error);

            return res.status(500).json({
                message:
                    "No se pudo restablecer el progreso."
            });
        }
    }
);

async function startServer() {
    if (
        !process.env.MONGODB_URI ||
        !process.env.JWT_SECRET
    ) {
        throw new Error(
            "Faltan MONGODB_URI o JWT_SECRET en .env."
        );
    }

    const client =
        new MongoClient(process.env.MONGODB_URI);

    await client.connect();

    const database =
        client.db(process.env.DB_NAME || "nexcode");

    usersCollection =
        database.collection("users");

    await usersCollection.createIndex(
        { email: 1 },
        { unique: true }
    );

    app.listen(port, () => {
        console.log(
            `API NexCode ejecutándose en http://localhost:${port}`
        );
    });
}

startServer().catch(error => {
    console.error("No se pudo iniciar el servidor:");
    console.error(error);
    process.exit(1);
});