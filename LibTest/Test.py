import os
import json
import re
import sqlite3
import subprocess
import uuid
import hmac
import hashlib
import datetime


DEBUG_PATH = "../src/Conduit/bin/Debug/netcoreapp2.2/Conduit.dll"
RELEASE_PATH = "../src/Conduit/bin/Release/netcoreapp2.2/Conduit.dll"
PATH_CONFIG = "."

if not os.path.isfile('./Conduit.dll'):
    if os.path.isfile(DEBUG_PATH):
        PATH_CONFIG = "../src/Conduit/bin/Debug/netcoreapp2.2"
    else:
        if os.path.isfile(RELEASE_PATH):
            PATH_CONFIG = "../src/Conduit/bin/Release/netcoreapp2.2"
        else:
            print("Cannot find Conduit.dll")
            input("Exiting script...")
            exit()


PRE_TEST_STRING = "cd " + PATH_CONFIG + " && dotnet Conduit.dll "
END_TEST_STRING = " --test"
DB_NAME = PATH_CONFIG + "/" +"realworldtest.db"
SECRET_KEY = 'realworld'

WillTest = {
    "User": 1,
    "Article": 1,
    "Comment": 1,
    "Favorite": 1,
    "Follow": 1,
    "Profile": 1,
    "Tag": 1
}

TableScheme = {
    "Persons": {
        "Id": 0,
        "Username": 1,
        "Email": 2,
        "Bio": 3,
        "Image": 4,
        "Hash": 5,
        "Salt": 6
    },
    "Articles": {
        "ArticleId": 0,
        "Slug": 1,
        "Title": 2,
        "Description": 3,
        "Body": 4,
        "AuthorPersonId": 5,
    },
    "Comment": {
        "CommentId": 0,
        "Body": 1,
        "AuthorId": 2,
        "ArticleId": 3,
    }
}

user01 = {
    "Username": "user01",
    "Password": "password",
    "Email": "estemail1@emai.co"
}

article01 = {
    "Title": "arTitle",
    "Slug": "arTitle",
    "Description": "title des",
    "Body": "ar body",
    "Tag1": "Tag01",
    "Tag2": "Tag02",
    "AuthorName": "user01"
}


# region FUNCTION

def insert_user(username, password, email, uid=1000):
    salt = uuid.uuid4().bytes
    byte = password.encode()
    hash_password = hmac.new(SECRET_KEY.encode(), byte + salt, hashlib.sha512)
    hash_password = hash_password.digest()

    i_connect = sqlite3.connect(DB_NAME)
    i_cur = i_connect.cursor()
    i_cur.execute("INSERT INTO Persons VALUES ( ?, ?, ?, NULL, NULL, ?, ?)",
                  (uid, username, email, memoryview(hash_password), memoryview(salt)))
    i_connect.commit()
    i_connect.close()
    return


def insert_article(slug, title, des, body, author, uid=1000):
    a_connect = sqlite3.connect(DB_NAME)
    a_cur = a_connect.cursor()
    a_cur.execute('INSERT INTO Articles VALUES ( ?, ?, ?, ?, ?, ?, ?, ?)',
                  (uid, slug, title, des, body, author, datetime.datetime.now(), datetime.datetime.now()))
    a_connect.commit()
    a_connect.close()
    return


def execute_command(command):
    return os.popen(PRE_TEST_STRING + command + END_TEST_STRING).read()


def get_json(source):
    matches = re.finditer(r"\{(.*)\}", source, re.MULTILINE | re.DOTALL)
    for matchNum, match in enumerate(matches):
        for groupNum in range(0, len(match.groups())):
            return json.loads("{" + match.group(1) + "}")


def reset_db():
    subprocess.Popen(PRE_TEST_STRING + "--test --resetdb", stdout=subprocess.PIPE, shell=True).wait()
    return


# endregion


def user_test():
    user_test_result = []

    # region Test create user
    reset_db()
    user02 = {
        "Username": "user01",
        "Password": "password",
        "Email": "estemail2@emai.co"
    }
    user03 = {
        "Username": "user02",
        "Password": "password",
        "Email": "estemail1@emai.co"
    }

    # create default user
    print("Running create user test", end="")
    reset_db()
    connect = sqlite3.connect(DB_NAME)
    cursor = connect.cursor()
    execute_command('create-user --username=\"{0}\" --password=\"{1}\" --email=\"{2}\"'. \
                    format(user01["Username"], user01["Password"], user01["Email"]))

    user = cursor.execute('SELECT * FROM Persons WHERE Username=?', (user01["Username"],)).fetchall()
    failed = 0
    if len(user) != 1:
        failed = 1
        user_test_result.append("ERROR: USER NOT FOUND AFTER CREATE")
    if not failed:
        if user[0][TableScheme["Persons"]["Email"]] != user01["Email"]:
            failed = 1
            user_test_result.append("ERROR: USER DATA NOT MATCH AFTER CREATE")

    # create bug user
    execute_command('create-user --username=\"{0}\" --password=\"{1}\" --email=\"{2}\"'. \
                    format(user02["Username"], user02["Password"], user02["Email"]))
    execute_command('create-user --username=\"{0}\" --password=\"{1}\" --email=\"{2}\"'. \
                    format(user03["Username"], user03["Password"], user03["Email"]))

    user = cursor.execute('SELECT * FROM Persons').fetchall()
    if len(user) != 1:
        failed = 1
        user_test_result.append("ERROR: USERNAME OR EMAIL CANNOT SAME")
    if failed:
        print(": FAILED")
    else:
        print(": SUCCESS")
    connect.close()
    # endregion

    # region Test get user
    reset_db()
    print("Running get user test", end="")
    insert_user(user01["Username"], user01["Password"], user01["Email"])

    user_data = execute_command("get-user --username=\"{0}\"".format(user01["Username"]))
    if get_json(user_data)["User"]["Email"] != user01["Email"]:
        print(": FAILED")
        user_test_result.append("ERROR: GET USER DATA MISMATCH")
    else:
        print(": SUCCESS")
    # endregion

    # region Test login
    print("Running login test", end="")
    failed = 0
    user_data = execute_command("login --email=\"{0}\" --password=\"{1}\"".format(user01["Email"], user01["Password"]))
    data = get_json(user_data)
    if 'Error' in data:
        failed = 1
        user_test_result.append("ERROR: UNABLE LOGIN")
    if 'User' in data:
        if data["User"]["Username"] != user01["Username"]:
            failed = 1
            user_test_result.append("ERROR: LOGIN RETURN MISMATCH USER DATA")
    if failed:
        print(": FAILED")
    else:
        print(": SUCCESS")

    # endregion

    # region Test edit user
    reset_db()
    insert_user(user01["Username"], user01["Password"], user01["Email"])

    connect = sqlite3.connect(DB_NAME)
    cursor = connect.cursor()
    print("Running edit user test", end="")
    failed = 0
    new_user01 = {
        "Username": "brandnewuser",
        "Password": "newpassword",
        "Email": "estemail1@emai.co",
        "Bio": "doctor"
    }
    execute_command("update-user --username=\"{0}\" --new-username=\"{1}\" --bio=\"{2}\" --password=\"{3}\"". \
                    format(user01["Username"], new_user01["Username"], new_user01["Bio"], new_user01["Password"]))
    user = cursor.execute("SELECT * FROM Persons WHERE Username='{0}'".format(new_user01["Username"])).fetchall()
    if len(user) == 0:
        failed = 1
        user_test_result.append("ERROR: UPDATE USER FAILED")
    if user[0][TableScheme["Persons"]["Bio"]] != new_user01["Bio"]:
        failed = 1
        user_test_result.append("ERROR: UPDATE USER DATA MISMATCH")
    if not failed:
        print(": SUCCESS")
    else:
        print(": FAILED")
    connect.commit()
    connect.close()
    # endregion

    if len(user_test_result) <= 0:
        user_test_result.append(" --- User features test successful --- ")
    else:
        user_test_result.append(" --- User features contain %s vulnerabilities --- " % len(user_test_result))
    return user_test_result


def article_test():
    article_test_result = []

    # region Test create article
    print("Running test create article", end="")
    reset_db()
    failed = 0
    insert_user(user01["Username"], user01["Password"], user01["Email"])
    execute_command("create-article --title=\"{0}\" --description=\"{1}\" --body=\"{2}\" --tag-list=\"{3}\","
                    "\"{4}\" --username=\"{5}\"".format(article01["Title"], article01["Description"], article01["Body"],
                                                        article01["Tag1"], article01["Tag2"], article01["AuthorName"]))

    connect = sqlite3.connect(DB_NAME)
    cursor = connect.cursor()

    c_res = cursor.execute("SELECT * FROM Articles").fetchone()
    if c_res[TableScheme["Articles"]["Title"]] != article01["Title"] or \
            c_res[TableScheme["Articles"]["Body"]] != article01["Body"] or \
            c_res[TableScheme["Articles"]["Body"]] != article01["Body"]:
        failed = 1
        article_test_result.append("ERROR: ARTICLE NOT CREATE OR CREATE DATA MISMATCH")

    c_res = cursor.execute('SELECT * FROM Tags WHERE TagId=? or TagId=?', (article01["Tag1"], article01["Tag2"]))\
        .fetchall()
    if len(c_res) != 2:
        failed = 1
        article_test_result.append("ERROR: TAGS NOT CREATE DURING CREATION OF ARTICLE")
    if failed:
        print(": FAILED")
    else:
        print(": SUCCESS")
    connect.close()
    # endregion

    # region Test list articles
    print("Running test list articles", end="")
    reset_db()
    user_id = 101
    insert_user(user01["Username"], user01["Password"], user01["Email"], user_id)
    connect = sqlite3.connect(DB_NAME)
    cursor = connect.cursor()
    cursor.execute('INSERT INTO Articles VALUES ( ?, ?, ?, ?, ?, ?, ?, ?)',
                   ("1", "Title-1231", "Title 1", "ar 1 des", " ar 1 body", str(user_id), datetime.datetime.now(),
                    datetime.datetime.now()))
    cursor.execute('INSERT INTO Articles VALUES ( ?, ?, ?, ?, ?, ?, ?, ?)',
                   ("2", "Title-1331", "Title 2", "ar 2 des", " ar 2 body", str(user_id), datetime.datetime.now(),
                    datetime.datetime.now()))
    connect.commit()
    connect.close()

    article = get_json(execute_command("list-articles"))
    if 'Articles' in article and 'ArticlesCount' in article:
        if len(article["Articles"]) != 2:
            print(": FAILED")
            article_test_result.append("ERROR: LIST ARTICLES DATA MISMATCH")
        else:
            print(": SUCCESS")
    else:
        print(": FAILED")
        article_test_result.append("ERROR: UNABLE TO LIST ARTICLE")
    # endregion

    # region Test get articles
    print("Running test get articles", end="")
    reset_db()
    user_id = 101
    failed = 0
    insert_user(user01["Username"], user01["Password"], user01["Email"], user_id)
    insert_user("kingdom", "framework", "email112", user_id + 1)
    connect = sqlite3.connect(DB_NAME)
    cursor = connect.cursor()
    cursor.execute('INSERT INTO Articles VALUES ( ?, ?, ?, ?, ?, ?, ?, ?)',
                   ("1", "Title-1231", "Title 1", "ar 1 des", " ar 1 body", str(user_id), datetime.datetime.now(),
                    datetime.datetime.now()))
    cursor.execute('INSERT INTO Articles VALUES ( ?, ?, ?, ?, ?, ?, ?, ?)',
                   ("2", "Title-1331", "Title 2", "ar 2 des", " ar 2 body", str(user_id), datetime.datetime.now(),
                    datetime.datetime.now()))
    cursor.execute('INSERT INTO Articles VALUES ( ?, ?, ?, ?, ?, ?, ?, ?)',
                   ("3", "Title-1431", "Title 3", "ar 3 des", " ar 2 body", str(user_id + 1), datetime.datetime.now(),
                    datetime.datetime.now()))
    connect.commit()
    connect.close()

    articles_user1 = get_json(execute_command("get-articles --author=\"{0}\"".format(user01["Username"])))
    articles_user2 = get_json(execute_command("get-articles --author=\"{0}\"".format("kingdom")))
    if "ArticlesCount" in articles_user1:
        if articles_user1["ArticlesCount"] != 2:
            failed = 1
    if "ArticlesCount" in articles_user2:
        if articles_user2["ArticlesCount"] != 1:
            failed = 1
    if failed:
        print(": FAILED")
        article_test_result.append("ERROR: GET ARTICLES RETURN DATA NOT MATCH")
    else:
        print(": SUCCESS")
    # endregion

    # region Test get article
    print("Running test get article", end="")
    reset_db()
    user_id = 100
    failed = 0
    f_slug = "article1-slug"
    s_slug = "article2-slug"
    insert_user(user01["Username"], user01["Password"], user01["Email"], user_id)
    insert_article(f_slug, "title 1", "des 1", "body 1", str(user_id), 1)
    insert_article(s_slug, "title 2", "des 2", "body 2", str(user_id), 2)

    article = get_json(execute_command("get-article --slug=\"{0}\"".format(f_slug)))
    if "Article" in article:
        try:
            if article["Article"]["Title"] != "title 1" and article["Article"]["Description"] != "des 1":
                failed = 1
                article_test_result.append("ERROR: GET ARTICLE RETURN DATA MISMATCH")
        except:
            failed = 1
            print("ERROR: UNABLE TO GET ARTICLE DATA")

    if failed:
        print(": FAILED")
    else:
        print(": SUCCESS")
    # endregion

    # region Test edit article
    print("Running test edit article", end="")
    reset_db()
    user_id = 100
    failed = 0
    insert_user(user01["Username"], user01["Password"], user01["Email"], user_id)
    f_slug = "article1-slug"

    insert_article(f_slug, "title 1", "des 1", "body 1", str(user_id), 1)
    execute_command("edit-article --slug=\"{0}\" --title=\"{1}\" --body=\"{2}\"".format(f_slug, "new title", "boy"))

    connect = sqlite3.connect(DB_NAME)
    cursor = connect.cursor()
    article = cursor.execute("SELECT * FROM Articles WHERE ArticleId=1").fetchone()
    if article[TableScheme["Articles"]["Title"]] != "new title" or \
            article[TableScheme["Articles"]["Body"]] != "boy":
        failed = 0
        article_test_result.append("ERROR: ARTICLE NOT UPDATE")
    if failed:
        print(": FAILED")
    else:
        print(": SUCCESS")
    connect.close()
    # endregion

    # region Test delete article
    print("Running test delete article", end="")
    reset_db()
    user_id = 100
    insert_user(user01["Username"], user01["Password"], user01["Email"], user_id)
    f_slug_1 = "article1-slug"

    insert_article(f_slug_1, "title 1", "des 1", "body 1", str(user_id), 1)
    execute_command("delete-article --slug=\"{0}\"".format(f_slug_1))

    connect = sqlite3.connect(DB_NAME)
    cursor = connect.cursor()

    (x_count,) = cursor.execute('SELECT COUNT(*) FROM Articles').fetchone()

    if x_count != 0:
        print(": FAILED")
        article_test_result.append("ERROR: ARTICLE NOT DELETE")
    else:
        print(": SUCCESS")

    connect.close()

    # endregion

    if len(article_test_result) == 0:
        article_test_result.append(" --- Article features test successful --- ")
    else:
        article_test_result.append(" --- Article features contain %s vulnerabilities --- " % len(article_test_result))

    return article_test_result


def comment_test():
    comment_test_result = []

    # region Test create comment
    print("Running test create comment", end="")
    reset_db()
    user_id = 100
    arti_id = 12
    insert_user(user01["Username"], user01["Password"], user01["Email"], user_id)
    insert_article(article01["Slug"], article01["Title"], article01["Description"],
                   article01["Body"], user_id, arti_id)

    execute_command("create-comment --username=\"{0}\" --body=\"{1}\" --slug=\"{2}\"".format(user01["Username"],
                                                                                             "this is my comment",
                                                                                             article01["Slug"]))
    connect = sqlite3.connect(DB_NAME)
    cursor = connect.cursor()

    comment = cursor.execute("SELECT * FROM Comments WHERE Body=? AND ArticleId=? AND AuthorId=?",
                             ("this is my comment", str(arti_id), user_id)).fetchone()

    if comment is None:
        print(": FAILED")
        comment_test_result.append("ERROR: COMMENT NOT CREATE")
    else:
        print(": SUCCESS")
    connect.close()
    # endregion

    # region Test get comments
    print("Running test get comments", end="")
    reset_db()
    user_id = 100
    arti_id = 12
    insert_user(user01["Username"], user01["Password"], user01["Email"], user_id)
    insert_article(article01["Slug"], article01["Title"], article01["Description"],
                   article01["Body"], user_id, arti_id)

    connect = sqlite3.connect(DB_NAME)
    cursor = connect.cursor()

    cursor.execute("INSERT INTO Comments VALUES (?, ?, ?, ?, ?, ?)", (1, "the body 1", str(user_id), str(arti_id),
                                                                      datetime.datetime.now(), datetime.datetime.now()))
    cursor.execute("INSERT INTO Comments VALUES (?, ?, ?, ?, ?, ?)", (2, "the body 2", str(user_id), str(arti_id),
                                                                      datetime.datetime.now(), datetime.datetime.now()))

    connect.commit()
    connect.close()

    a_comment = get_json(execute_command("list-comments --slug=\"{0}\"".format(article01["Slug"])))

    if a_comment is not None and 'Comments' in a_comment:
        if len(a_comment["Comments"]) != 2:
            print(": FAILED")
            comment_test_result.append("ERROR: GET COMMENTS RETURN MISMATCH DATA")
        else:
            print(": SUCCESS")
    else:
        if a_comment is None:
            print(": FAILED")
            comment_test_result.append("ERROR: GET COMMENTS RETURN MISMATCH DATA")
        else:
            print(": UNKNOWN RETURN")
    # endregion

    # region Test delete comment
    print("Running test delete comment", end="")
    reset_db()
    user_id = 100
    arti_id = 12
    insert_user(user01["Username"], user01["Password"], user01["Email"], user_id)
    insert_article(article01["Slug"], article01["Title"], article01["Description"],
                   article01["Body"], user_id, arti_id)

    connect = sqlite3.connect(DB_NAME)
    cursor = connect.cursor()

    cursor.execute("INSERT INTO Comments VALUES (?, ?, ?, ?, ?, ?)", (1, "the body 1", str(user_id), str(arti_id),
                                                                      datetime.datetime.now(), datetime.datetime.now()))
    cursor.execute("INSERT INTO Comments VALUES (?, ?, ?, ?, ?, ?)", (2, "the body 2", str(user_id), str(arti_id),
                                                                      datetime.datetime.now(), datetime.datetime.now()))

    connect.commit()
    connect.close()

    execute_command("delete-comment --slug=\"{0}\" --id=\"{1}\"".format(article01["Slug"], 1))
    execute_command("delete-comment --slug=\"{0}\" --id=\"{1}\"".format(article01["Slug"], 2))

    connect = sqlite3.connect(DB_NAME)
    cursor = connect.cursor()
    c_com = cursor.execute("SELECT * FROM Comments WHERE ArticleId=? and (CommentId=? or CommentId=?)",
                           (str(arti_id), 1, 2)).fetchone()

    if c_com is not None:
        print(": FAILED")
        comment_test_result.append("ERROR: COMMENTS NOT DELETE")
    else:
        print(": SUCCESS")
    connect.close()

    # endregion
    if len(comment_test_result) == 0:
        comment_test_result.append(" --- Comment features test successful --- ")
    else:
        comment_test_result.append(" --- Comment features contain %s vulnerabilities --- " % len(comment_test_result))
    return comment_test_result


def favorite_test():
    favorite_test_result = []

    # region Test add favorite
    print("Running test add favorite", end="")
    reset_db()
    user_id = 100
    arti_id = 12
    insert_user(user01["Username"], user01["Password"], user01["Email"], user_id)
    insert_article(article01["Slug"], article01["Title"], article01["Description"],
                   article01["Body"], user_id, arti_id)

    execute_command("add-favorite --username=\"{0}\" --slug=\"{1}\"".format(user01["Username"], article01["Slug"]))

    connect = sqlite3.connect(DB_NAME)
    cursor = connect.cursor()

    c_res = cursor.execute("SELECT * FROM ArticleFavorites WHERE ArticleId=? and PersonId=?",
                           (arti_id, user_id)).fetchone()

    connect.close()

    if c_res is None:
        print(": FAILED")
        favorite_test_result.append("ERROR: LIKE ARTICLE FAILED")
    else:
        print(": SUCCESS")
    # endregion

    # region Test delete favorite
    print("Running test add favorite", end="")
    reset_db()
    user_id = 100
    arti_id = 12
    insert_user(user01["Username"], user01["Password"], user01["Email"], user_id)
    insert_article(article01["Slug"], article01["Title"], article01["Description"],
                   article01["Body"], user_id, arti_id)

    connect = sqlite3.connect(DB_NAME)
    cursor = connect.cursor()

    cursor.execute("INSERT INTO ArticleFavorites VALUES(?, ?)", (str(arti_id), str(user_id)))

    connect.commit()
    connect.close()

    execute_command("delete-favorite --username=\"{0}\" --slug=\"{1}\"".format(user01["Username"], article01["Slug"]))

    connect = sqlite3.connect(DB_NAME)
    cursor = connect.cursor()

    c_res = cursor.execute("SELECT * FROM ArticleFavorites WHERE ArticleId=? and PersonId=?",
                           (str(arti_id), str(user_id))).fetchall()

    if c_res is not None and len(c_res) > 0:
        print(": FAILED")
        favorite_test_result.append("ERROR: FAVORITE NOT DELETE")
    else:
        print(": SUCCESS")
    connect.close()
    # endregion

    if len(favorite_test_result) == 0:
        favorite_test_result.append(" --- Favorite features test successful --- ")
    else:
        favorite_test_result.append(
            " --- Favorite features contain %s vulnerabilities --- " % (len(favorite_test_result)))
    return favorite_test_result


def follow_test():
    follow_test_result = []
    user02 = {
        "Username": "user02",
        "Password": "userpassword",
        "Email": "useremail@emai.co"
    }
    # region Test follow
    print("Running follow user test", end="")
    reset_db()
    user01_id = 1
    user02_id = 2
    insert_user(user01["Username"], user01["Password"], user01["Email"], user01_id)
    insert_user(user02["Username"], user02["Password"], user02["Email"], user02_id)

    execute_command("follow --username=\"{0}\" --target=\"{1}\"".format(user01["Username"], user02["Username"]))

    connect = sqlite3.connect(DB_NAME)
    cursor = connect.cursor()

    c_res = cursor.execute("SELECT * FROM FollowedPeople WHERE ObserverId=? and TargetId=?",
                           (str(user01_id), str(user02_id))).fetchone()
    if c_res is None:
        print(": FAILED")
        follow_test_result.append("ERROR: CANNOT FOLLOW USER")
    else:
        print(": SUCCESS")

    connect.close()
    # endregion

    # region Test unfollow
    print("Running test unfollow user", end="")
    reset_db()
    user01_id = 1
    user02_id = 2
    insert_user(user01["Username"], user01["Password"], user01["Email"], user01_id)
    insert_user(user02["Username"], user02["Password"], user02["Email"], user02_id)

    connect = sqlite3.connect(DB_NAME)
    cursor = connect.cursor()

    cursor.execute("INSERT INTO FollowedPeople VALUES(?, ?)", (str(user01_id), str(user02_id)))

    connect.commit()
    connect.close()

    execute_command("unfollow --username=\"{0}\" --target=\"{1}\"".format(user01["Username"], user02["Username"]))

    connect = sqlite3.connect(DB_NAME)
    cursor = connect.cursor()

    c_res = cursor.execute("SELECT * FROM FollowedPeople WHERE ObserverId=? and TargetId=?",
                           (str(user01_id), str(user02_id))).fetchone()

    if c_res is not None:
        print(": FAILED")
        follow_test_result.append("ERROR: CANNOT UNFOLLOW PEOPLE")
    else:
        print(": SUCCESS")
    connect.close()
    # endregion

    if len(follow_test_result) == 0:
        follow_test_result.append(" --- Follow features test successful --- ")
    else:
        follow_test_result.append(" --- Follow features contain %s vulnerabilities" % len(follow_test_result))
    return follow_test_result


def profile_test():
    profile_test_result = []
    user02 = {
        "Username": "user02",
        "Password": "userpassword",
        "Email": "useremail@emai.co"
    }
    user03 = {
        "Username": "user03",
        "Password": "user03password",
        "Email": "user03email@emai.co"
    }

    # region Test get profile
    print("Running test get profile", end=""),
    reset_db()
    failed = 0
    user01_id = 1
    user02_id = 2
    user03_id = 3
    insert_user(user01["Username"], user01["Password"], user01["Email"], user01_id)
    insert_user(user02["Username"], user02["Password"], user02["Email"], user02_id)
    insert_user(user03["Username"], user03["Password"], user03["Email"], user03_id)

    connect = sqlite3.connect(DB_NAME)
    cursor = connect.cursor()

    cursor.execute("INSERT INTO FollowedPeople VALUES (?, ?)", (str(user01_id), str(user02_id)))

    connect.commit()
    connect.close()

    e_res_f_user = get_json(execute_command('get-profile --username="{0}" --target="{1}"'.
                                            format(user01["Username"], user02["Username"])))
    e_res_s_user = get_json(execute_command('get-profile --username="{0}" --target="{1}"'.
                                            format(user01["Username"], user03["Username"])))

    try:
        if ('User' in e_res_f_user and e_res_f_user["User"] == "not found") or \
                ('User' in e_res_s_user and e_res_s_user["User"] == "not found"):
            failed = 1
            profile_test_result.append("ERROR: CANNOT FIND TARGET USER")
        if (('Profile' in e_res_f_user and 'following' in e_res_f_user)
            and (e_res_f_user["Profile"] != user02["Username"] or e_res_f_user["following"] is False)) or \
                (('Profile' in e_res_s_user and 'following' in e_res_s_user)
                 and (e_res_s_user["Profile"] != user02["Username"] or e_res_s_user["following"] is True)):
            failed = 1
            profile_test_result.append("ERROR: FOUND WRONG USER OR RETURN MISMATCH DATA")
        if failed:
            print(": FAILED")
        else:
            print(": SUCCESS")
    except:
        print(": Cannot read return data")

    # endregion

    if len(profile_test_result) == 0:
        profile_test_result.append(" --- Follow features test successful --- ")
    else:
        profile_test_result.append(" --- Follow features contain %s vulnerabilities --- " % len(profile_test_result))
    return profile_test_result


def tag_test():
    tag_test_result = []

    # region Test list tags
    print("Running test list tags", end="")
    reset_db()

    connect = sqlite3.connect(DB_NAME)
    cursor = connect.cursor()

    cursor.execute('INSERT INTO Tags VALUES("tag1")')
    cursor.execute('INSERT INTO Tags VALUES("tag2")')
    cursor.execute('INSERT INTO Tags VALUES("tag1-old")')

    connect.commit()
    connect.close()

    e_res = get_json(execute_command('list-tags'))

    try:
        if 'Tags' not in e_res or len(e_res["Tags"]) != 3:
            print(": FAILED")
            tag_test_result.append("ERROR: LIST TAGS RETURN DATA MISMATCH")
        else:
            print(": SUCCESS")
    except:
        print(": unable to read return data")

    # endregion
    if len(tag_test_result) == 0:
        tag_test_result.append(" --- Tag features test successful --- ")
    else:
        tag_test_result.append(" --- Tag features contain %s vulnerabilities --- " % len(tag_test_result))
    return tag_test_result

print("*** Script started")
print("* Lib path => %s/Conduit.dll" % PATH_CONFIG, end="\n************\n")

if WillTest["User"]:
    for x in user_test():
        print(x)

if WillTest["Article"]:
    for x in article_test():
        print(x)

if WillTest["Comment"]:
    for x in comment_test():
        print(x)

if WillTest["Favorite"]:
    for x in favorite_test():
        print(x)

if WillTest["Follow"]:
    for x in follow_test():
        print(x)

if WillTest["Profile"]:
    for x in profile_test():
        print(x)

if WillTest["Tag"]:
    for x in tag_test():
        print(x)

input("Press any key...")
