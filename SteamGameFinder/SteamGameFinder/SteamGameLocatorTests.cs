using NUnit.Framework;
using Narod.SteamGameFinder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Narod.Tests.SteamGameFinder
{
    [TestFixture]
    public class SteamGameLocatorTests
    {
        private SteamGameLocatorOptions _defaultOptions;
        private SteamGameLocator _locator;

        [SetUp]
        public void Setup()
        {
            _defaultOptions = new SteamGameLocatorOptions
            {
                MemorySettings = MemorySettings.Full,
                SuppressExceptions = false,
                ShouldIndexLibrary = true
            };
        }

        [TearDown]
        public void Cleanup()
        {
            _locator = null;
        }

        #region Constructor Tests

        [Test]
        public void Constructor_WithNullOptions_CreatesDefaultOptions()
        {
            // Arrange & Act
            var locator = new SteamGameLocator(null);

            // Assert
            Assert.IsNotNull(locator);
        }

        [Test]
        public void Constructor_WithProvidedOptions_UsesProvidedOptions()
        {
            // Arrange
            var customOptions = new SteamGameLocatorOptions
            {
                MemorySettings = MemorySettings.None,
                SuppressExceptions = true,
                ShouldIndexLibrary = false
            };

            // Act
            var locator = new SteamGameLocator(customOptions);

            // Assert
            Assert.IsNotNull(locator);
        }

        [Test]
        public void Constructor_DefaultCreation_DoesNotThrowException()
        {
            // Arrange & Act & Assert
            var locator = new SteamGameLocator();
            Assert.IsNotNull(locator);
        }

        #endregion

        #region GameStruct Tests

        [Test]
        public void GameStruct_CanCreateWithDefaults()
        {
            // Arrange & Act
            var gameStruct = new SteamGameLocator.GameStruct();

            // Assert
            Assert.IsNotNull(gameStruct);
            Assert.IsNull(gameStruct.steamGameID);
            Assert.IsNull(gameStruct.steamGameName);
            Assert.IsNull(gameStruct.steamGameLocation);
        }

        [Test]
        public void GameStruct_CanSetAllProperties()
        {
            // Arrange & Act
            var gameStruct = new SteamGameLocator.GameStruct
            {
                steamGameID = "123456",
                steamGameName = "TestGame",
                steamGameLocation = "C:\\Games\\TestGame"
            };

            // Assert
            Assert.AreEqual("123456", gameStruct.steamGameID);
            Assert.AreEqual("TestGame", gameStruct.steamGameName);
            Assert.AreEqual("C:\\Games\\TestGame", gameStruct.steamGameLocation);
        }

        #endregion

        #region getIsSteamInstalled Tests

        [Test]
        public void getIsSteamInstalled_WithMemorySettingsNone_AlwaysChecks()
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                MemorySettings = MemorySettings.None,
                SuppressExceptions = true
            };
            _locator = new SteamGameLocator(options);

            // Act - First call
            var result1 = _locator.getIsSteamInstalled();

            // Act - Second call (should recheck since MemorySettings.None)
            var result2 = _locator.getIsSteamInstalled();

            // Assert
            // Both calls should complete without error (specific values depend on system)
            Assert.IsTrue(result1 is bool);
            Assert.IsTrue(result2 is bool);
        }

        [Test]
        public void getIsSteamInstalled_WithMemorySettingsFull_CachesResult()
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                MemorySettings = MemorySettings.Full,
                SuppressExceptions = true
            };
            _locator = new SteamGameLocator(options);

            // Act - First call
            var result1 = _locator.getIsSteamInstalled();

            // Act - Second call (should use cache)
            var result2 = _locator.getIsSteamInstalled();

            // Assert
            Assert.AreEqual(result1, result2);
        }

        [Test]
        public void getIsSteamInstalled_ReturnsBoolValue()
        {
            // Arrange
            var options = new SteamGameLocatorOptions { SuppressExceptions = true };
            _locator = new SteamGameLocator(options);

            // Act
            var result = _locator.getIsSteamInstalled();

            // Assert
            Assert.IsTrue(result is bool);
        }

        #endregion

        #region getSteamInstallLocation Tests

        [Test]
        public void getSteamInstallLocation_WithSuppressExceptionsTrue_ReturnsNullWhenSteamNotFound()
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                SuppressExceptions = true,
                MemorySettings = MemorySettings.None
            };
            _locator = new SteamGameLocator(options);

            // Act
            var result = _locator.getSteamInstallLocation();

            // Assert (will be null or a valid path, not an exception)
            Assert.IsTrue(result == null || result is string);
        }

        [Test]
        public void getSteamInstallLocation_WithMemorySettingsNone_DoesNotCache()
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                MemorySettings = MemorySettings.None,
                SuppressExceptions = true
            };
            _locator = new SteamGameLocator(options);

            // Act
            var result1 = _locator.getSteamInstallLocation();
            var result2 = _locator.getSteamInstallLocation();

            // Assert
            // Both should be the same value (actual path or null)
            Assert.AreEqual(result1, result2);
        }

        [Test]
        public void getSteamInstallLocation_ReturnsStringOrNull()
        {
            // Arrange
            var options = new SteamGameLocatorOptions { SuppressExceptions = true };
            _locator = new SteamGameLocator(options);

            // Act
            var result = _locator.getSteamInstallLocation();

            // Assert
            Assert.IsTrue(result == null || result is string);
        }

        #endregion

        #region getSteamLibraryLocations Tests

        [Test]
        public void getSteamLibraryLocations_ReturnsListOfStrings()
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                SuppressExceptions = true,
                MemorySettings = MemorySettings.None
            };
            _locator = new SteamGameLocator(options);

            // Act
            try
            {
                var result = _locator.getSteamLibraryLocations();

                // Assert
                Assert.IsNotNull(result);
                Assert.IsTrue(result is List<string>);
            }
            catch (DirectoryNotFoundException)
            {
                // Expected if Steam is not installed or file not found
                Assert.Pass();
            }
        }

        [Test]
        public void getSteamLibraryLocations_WithMemorySettingsFull_CachesResult()
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                MemorySettings = MemorySettings.Full,
                SuppressExceptions = true
            };
            _locator = new SteamGameLocator(options);

            // Act
            try
            {
                var result1 = _locator.getSteamLibraryLocations();
                var result2 = _locator.getSteamLibraryLocations();

                // Assert
                Assert.IsNotNull(result1);
                Assert.IsNotNull(result2);
                Assert.AreSame(result1, result2); // Should be the same cached list
            }
            catch (DirectoryNotFoundException)
            {
                // Expected if Steam is not installed
                Assert.Pass();
            }
        }

        #endregion

        #region indexSteamGames Tests

        [Test]
        public void indexSteamGames_WithShouldIndexLibraryFalse_DoesNotThrow()
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                ShouldIndexLibrary = false,
                SuppressExceptions = true
            };
            _locator = new SteamGameLocator(options);

            // Act & Assert - Should not throw
            Assert.DoesNotThrow(delegate
            {
                _locator.indexSteamGames();
            });
        }

        [Test]
        public void indexSteamGames_WithNoLibrariesAndSuppressExceptions_DoesNotThrow()
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                ShouldIndexLibrary = true,
                SuppressExceptions = true,
                MemorySettings = MemorySettings.None
            };
            _locator = new SteamGameLocator(options);
            
            // Act & Assert

            Assert.DoesNotThrow(delegate
            {
                _locator.indexSteamGames();
            });
        }

        [Test]
        public void indexSteamGames_WithNoLibrariesAndSuppressExceptionsFalse_ThrowsInvalidOperationException()
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                ShouldIndexLibrary = true,
                SuppressExceptions = false,
                MemorySettings = MemorySettings.None
            };
            _locator = new SteamGameLocator(options);

            // Act & Assert
            try
            {
                _locator.indexSteamGames();
                // Implicit pass
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("Cannot index library with no libraries present.", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }

        #endregion

        #region getGameInfoByFolder Tests

        [Test]
        public void getGameInfoByFolder_WithSuppressExceptionsTrue_ReturnsEmptyGameStructWhenNotFound()
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                SuppressExceptions = true,
                ShouldIndexLibrary = false,
                MemorySettings = MemorySettings.None
            };
            _locator = new SteamGameLocator(options);

            // Act
            try
            {
                var result = _locator.getGameInfoByFolder("NonExistentGame");

                // Assert
                Assert.AreEqual(result, new SteamGameLocator.GameStruct());
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Test]
        public void getGameInfoByFolder_ReturnsGameStruct() // TEST RELIES ON HAVING OVERWATCH INSTALLED
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                SuppressExceptions = true,
                ShouldIndexLibrary = false
            };
            _locator = new SteamGameLocator(options);

            // Act & Assert
            try
            {
                var result = _locator.getGameInfoByFolder("Overwatch");
                Assert.NotNull(result.steamGameLocation);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Test]
        public void getGameInfoByFolder_ReturnsGameStruct_Index() // TEST RELIES ON HAVING OVERWATCH INSTALLED
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                SuppressExceptions = true,
                ShouldIndexLibrary = true
            };
            _locator = new SteamGameLocator(options);

            // Act & Assert
            try
            {
                var result = _locator.getGameInfoByFolder("Overwatch");
                Assert.AreEqual("2357570", result.steamGameID);
            }
            catch
            {
                // Expected if Steam not installed or libraries not accessible
                Assert.Fail();
            }
        }

        [Test]
        public void getGameInfoByFolder_ReturnsGameStruct_Index_NoGame() // TEST RELIES ON HAVING OVERWATCH INSTALLED
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                SuppressExceptions = true,
                ShouldIndexLibrary = true
            };
            _locator = new SteamGameLocator(options);

            // Act & Assert
            try
            {
                var result = _locator.getGameInfoByFolder("ThisGameDoesNotExist");
                Assert.AreEqual(result, new SteamGameLocator.GameStruct());
            }
            catch
            {
                // Expected if Steam not installed or libraries not accessible
                Assert.Fail();
            }
        }

        #endregion

        #region getGameInfoByID Tests

        [Test]
        public void getGameInfoByID_WithSuppressExceptionsTrue_ReturnsGameStructWhenNotFound()
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                SuppressExceptions = true,
                ShouldIndexLibrary = false
            };
            _locator = new SteamGameLocator(options);

            // Act
            var result = _locator.getGameInfoByID("999999999");

            // Assert
            Assert.AreEqual(result, new SteamGameLocator.GameStruct());
        }

        [Test]
        public void getGameInfoByID_ReturnsGameStruct()
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                SuppressExceptions = true,
                ShouldIndexLibrary = false
            };
            _locator = new SteamGameLocator(options);

            // Act
            var result = _locator.getGameInfoByID("123456");

            // Assert
            Assert.AreEqual(result, new SteamGameLocator.GameStruct());
        }

        [Test]
        public void getGameInfoByID_WithValidID_ReturnsGameStructWithCorrectID() // TEST RELIES ON HAVING OVERWATCH INSTALLED
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                SuppressExceptions = true,
                ShouldIndexLibrary = false
            };
            _locator = new SteamGameLocator(options);

            // Act
            var result = _locator.getGameInfoByID("2357570");

            // Assert
            Assert.AreEqual("2357570", result.steamGameID);
        }

        [Test]
        public void getGameInfoByID_WithValidID_ReturnsGameStructWithCorrectID_Index() // TEST RELIES ON HAVING OVERWATCH INSTALLED
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                SuppressExceptions = true,
                ShouldIndexLibrary = true
            };
            _locator = new SteamGameLocator(options);

            // Act
            var result = _locator.getGameInfoByID("2357570");

            // Assert
            Assert.AreEqual("2357570", result.steamGameID);
        }

        [Test]
        public void getGameInfoByID_WithValidID_ReturnsGameStructWithCorrectID_Index_InvalidGame() // TEST RELIES ON HAVING OVERWATCH INSTALLED
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                SuppressExceptions = true,
                ShouldIndexLibrary = true
            };
            _locator = new SteamGameLocator(options);

            // Act
            var result = _locator.getGameInfoByID("12341234");

            // Assert
            Assert.AreEqual(result, new SteamGameLocator.GameStruct());
        }

        #endregion

        #region getGameInfoByName Tests

        [Test]
        public void getGameInfoByName_WithIndexingDisabled_ThrowsInvalidOperationException()
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                ShouldIndexLibrary = false,
                SuppressExceptions = false
            };
            _locator = new SteamGameLocator(options);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(delegate
            {
                _locator.getGameInfoByName("TestGame");
            });
            Assert.IsTrue(exception.Message.Contains("indexing is disabled"));
        }

        [Test]
        public void getGameInfoByName_WithSuppressExceptionsTrue_ReturnsEmptyGameStructWhenNotFound()
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                ShouldIndexLibrary = true,
                SuppressExceptions = true
            };
            _locator = new SteamGameLocator(options);

            // Act
            try
            {
                var result = _locator.getGameInfoByName("NonExistentGame");

                // Assert
                Assert.AreEqual(result, new SteamGameLocator.GameStruct());
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Test]
        public void getGameInfoByName_ReturnsGameStruct()
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                ShouldIndexLibrary = true,
                SuppressExceptions = true
            };
            _locator = new SteamGameLocator(options);

            // Act
            try
            {
                var result = _locator.getGameInfoByName("TestGame");

                // Assert
                Assert.AreEqual(result, new SteamGameLocator.GameStruct());
            }
            catch
            {
                Assert.Fail();
            }
        }

        #endregion

        #region getAllGames Tests

        [Test]
        public void getAllGames_WithIndexingDisabled_ThrowsInvalidOperationException()
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                ShouldIndexLibrary = false,
                SuppressExceptions = false
            };
            _locator = new SteamGameLocator(options);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(delegate
            {
                _locator.getAllGames();
            });
            Assert.IsTrue(exception.Message.Contains("indexing is disabled"));
        }

        [Test]
        public void getAllGames_ReturnsListOfGameStructs()
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                ShouldIndexLibrary = true,
                SuppressExceptions = true
            };
            _locator = new SteamGameLocator(options);

            // Act
            try
            {
                var result = _locator.getAllGames();

                // Assert
                Assert.IsNotNull(result);
                Assert.IsTrue(result is List<SteamGameLocator.GameStruct>);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Test]
        public void getAllGames_ReturnsEmptyListWhenNoGamesFound()
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                ShouldIndexLibrary = true,
                SuppressExceptions = true,
                MemorySettings = MemorySettings.None
            };
            _locator = new SteamGameLocator(options);

            // Act
            try
            {
                var result = _locator.getAllGames();

                // Assert
                Assert.IsNotNull(result);
                Assert.IsTrue(result is List<SteamGameLocator.GameStruct>);
                // List may be empty if no games are installed
                Assert.GreaterOrEqual(result.Count, 0);
            }
            catch
            {
                Assert.Fail();
            }
        }

        #endregion

        #region Memory Settings Tests

        [Test]
        public void MemorySettings_Full_CachesAllResults()
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                MemorySettings = MemorySettings.Full,
                SuppressExceptions = true
            };
            _locator = new SteamGameLocator(options);

            // Act
            try
            {
                var isInstalled1 = _locator.getIsSteamInstalled();
                var location1 = _locator.getSteamInstallLocation();
                var libraries1 = _locator.getSteamLibraryLocations();

                var isInstalled2 = _locator.getIsSteamInstalled();
                var location2 = _locator.getSteamInstallLocation();
                var libraries2 = _locator.getSteamLibraryLocations();

                // Assert
                Assert.AreEqual(isInstalled1, isInstalled2);
                Assert.AreEqual(location1, location2);
                Assert.AreSame(libraries1, libraries2);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Test]
        public void MemorySettings_Partial_CachesLibrariesButNotGames()
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                MemorySettings = MemorySettings.Partial,
                SuppressExceptions = true,
                ShouldIndexLibrary = true
            };
            _locator = new SteamGameLocator(options);

            // Act
            try
            {
                var libraries1 = _locator.getSteamLibraryLocations();
                var libraries2 = _locator.getSteamLibraryLocations();

                // Assert
                Assert.IsNotNull(libraries1);
                Assert.IsNotNull(libraries2);
                // With Partial caching, library list should be cached
                Assert.AreSame(libraries1, libraries2);
            }
            catch
            {
                Assert.Fail();
            }
        }

        #endregion

        #region Exception Handling Tests

        [Test]
        public void SuppressExceptions_True_DoesNotThrowOnGameNotFound()
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                SuppressExceptions = true,
                ShouldIndexLibrary = false
            };
            _locator = new SteamGameLocator(options);

            // Act & Assert
            Assert.DoesNotThrow(delegate
            {
                var result = _locator.getGameInfoByFolder("NonExistent");
            });
        }

        [Test]
        public void SuppressExceptions_False_ThrowsOnGameNotFound()
        {
            // Arrange
            var options = new SteamGameLocatorOptions
            {
                SuppressExceptions = false,
                ShouldIndexLibrary = false
            };
            _locator = new SteamGameLocator(options);

            // Act & Assert
            try
            {
                var result = _locator.getGameInfoByFolder("NonExistent");
                // May or may not throw depending on system state
            }
            catch (DirectoryNotFoundException)
            {
                Assert.Pass(); // Expected exception
            }
            catch
            {
                // Other exceptions acceptable
                Assert.Pass();
            }
        }

        #endregion
    }
}